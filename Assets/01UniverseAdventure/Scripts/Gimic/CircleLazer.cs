using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CircleLazer : MonoBehaviour
{
    [Tooltip("サークルの半径"), SerializeField] float radius;
    [Tooltip("サークルレーザーが広がる最大半径"), SerializeField] float maxRadius;
    [Tooltip("円の頂点の数"), SerializeField] int numberOfVertex = 100;
    [Tooltip("円形のレーザーを描くlineRenderer"), SerializeField] LineRenderer lineRenderer;
    [Tooltip("レーザーに沿った当たり判定"), SerializeField] LazerCollider lazerCollider;
    [SerializeField] float spreadSpeed = 2f;
    [Tooltip("広がるだけか惑星に沿った動きをするか"), SerializeField] bool isOnlySpread;
    [SerializeField] GameObject planet;
    [Tooltip("レイザーの点が惑星から離れすぎたことを感知する最短距離"),SerializeField] float limitedDistance;
    [Tooltip("レイザーを消去するy軸方向の距離"), SerializeField] float destroySizeDistanceY;
    List<RaycastHit> hits = new List<RaycastHit>();
    float initializeRadius = 0.01f;
    /// <summary>
    /// 下がりながら広がっていくレーザーについて、最初は半径加算のみを行う。その後地面から離れていくにつれて
    /// 広がるスピードが遅くなってしまうので途中から広がりながら下がっていくようにする。ここで拡大のみを行う
    /// 場合このフラグがtrueになる
    /// </summary>
    bool isSpread;

    /// <summary>
    /// 下がりながら広がっていくレーザーのy座標4
    /// </summary>
    float posY;

    /// <summary>
    /// レーザーの各頂点の地面との距離
    /// </summary>
    Vector3 offset;


    // Start is called before the first frame update
    void Start()
    {
        radius = initializeRadius;
        //lineRenderはループさせるため作りたい頂点にもう一つ足した数だけ頂点を用意する
        lineRenderer.positionCount = numberOfVertex + 1;

        //コライダーの頂点を設定
        lazerCollider.SetColNumber(numberOfVertex);
    }

    // Update is called once per frame
    void Update()
    {
        //広がっていくのみのレーザー
        if (isOnlySpread)
        {
            OperateSpreadCircle();
        }

        //広がりながら下がっていくレーザー
        else
        {
            SwitchSpreadMode();
            OperateMoveThroghPlanetCircle();
        }

        DrawCircle();
    }

    /// <summary>
    /// レーザーの広がる最大半径とどのplanetに沿ってレーザーを動かすかを登録する関数
    /// </summary>
    /// <param name="setRadius">最大半径</param>
    /// <param name="setPlanet">対象のplanetオブジェクト</param>
    public void SetLazerSetting(float setRadius, GameObject setPlanet, float setLimitedDistance, float setStartDownSizeDistanceY)
    {
        maxRadius = setRadius;
        planet = setPlanet;
        limitedDistance = setLimitedDistance;
        destroySizeDistanceY = setStartDownSizeDistanceY;
    }

    /// <summary>
    /// 広がるだけのレーザの処理
    /// </summary>
    void OperateSpreadCircle()
    {
        //最大半径までレーザーを広げる
        if (radius < maxRadius)
        {
            radius += Time.deltaTime * spreadSpeed;
            posY = 0f;
        }

        //最大半径に達したらレーザーを消す
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 惑星に沿って動くレーザーについて広がる挙動のみを行う状態と下に下がっていきながら広がる挙動をする状態とを切り替える関数
    /// </summary>
    void SwitchSpreadMode()
    {
        //レーザの各頂点から惑星方向にレイを飛ばしたときのhit情報それぞれについて判定
        foreach(var hit in hits)
        {
            //それぞれのdistanceがlimitedDistanceより短い場合は拡大のみ
            if(hit.distance <= limitedDistance)
            {
                isSpread = true;
            }

            //円が広がりすぎたら下に下がっていく挙動に切り替える
            else
            {
                isSpread = false;
                break;
            }
        }
    }

    /// <summary>
    /// 広がりながら下がっていくレーザーの処理
    /// </summary>
    void OperateMoveThroghPlanetCircle()
    {
        //ただ拡大する
        if (isSpread)
        {
            radius += Time.deltaTime * spreadSpeed;
            posY = 0f;
        }

        //拡大しながら下降
        else
        {
            posY -= Time.deltaTime * spreadSpeed;
            if(posY >= -destroySizeDistanceY)
            {
                radius += Time.deltaTime * spreadSpeed;

            }

            else
            {
                Destroy(gameObject);
            }

        }
    }

    /// <summary>
    /// 円形のレーザーを描く関数
    /// </summary>
    void DrawCircle()
    {
        
        var segmentAngle = 360f / numberOfVertex;
        for (int i = 0; i <= numberOfVertex; i++)
        {
            Vector3 linePos = Vector3.zero;
            var angle = segmentAngle * i * Mathf.Deg2Rad;
            //x = cos(angle) × 半径   z = sin(angle) × 半径
            var x = Mathf.Cos(angle) * radius;
            var y = posY;
            var z = Mathf.Sin(angle) * radius;

            linePos = new Vector3(x, y, z);

            //広がるだけでなく下に降りていくタイプのレーザーの場合
            if (!isOnlySpread)
            {
                
                var worldPos = transform.position + linePos;

                //それぞれの頂点の座標が惑星上の地面からどれだけ離れたかを取得してその分移動させる
                offset = CheckGroundOffset(worldPos);
                var direction = (planet.transform.position - worldPos).normalized;

                //そのままだと地面に埋まってしまうので少しだけ上にあげる。
                var correction = -direction;

                linePos = linePos + offset + correction;
            }

            lineRenderer.SetPosition(i, linePos);
        }

    }

    /// <summary>
    /// 地面とレーザーの頂点との距離を取得する
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    Vector3 CheckGroundOffset(Vector3 origin)
    {
        var direction = (planet.transform.position - origin).normalized;
        RaycastHit hit;

        //各頂点の位置から惑星に向けてレイを飛ばし惑星の表面との間の距離を取得
        if (Physics.Raycast(origin, direction, out hit))
        {
            if (hit.collider.gameObject == planet)
            {
                hits.Add(hit);
                return direction * hit.distance;
            }

            //レイにプラネットが当たらない場合はoffsetの間をそのまま返す
            else
            {
                return offset;
            }

        }

        else
        {
            return offset;
        }
    }


}
