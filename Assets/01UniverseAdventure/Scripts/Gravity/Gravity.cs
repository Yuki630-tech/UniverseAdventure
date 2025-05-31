using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Threading;
using System;

public class Gravity : MonoBehaviour
{
    [Tooltip("重力を加えるためのRigidbody"), SerializeField] Rigidbody rb;
    [Tooltip("重力源となる惑星"), SerializeField] GameObject planetObj;
    [Tooltip("小惑星かを判断するための惑星についているコンポーネント"), SerializeField] Planet planet;

    [Tooltip("重力変換装置に触れたときに動けなくするために移動を制御するコンポーネントを取得しておく"), SerializeField] Movable movable;

    [Tooltip("鉄球の場合転がってほしいので上方向を固定しないようにしたい。そのため上方向固定のキャラクターかどうかを決定するフラグを用意しておく"), SerializeField]
    bool isCharacter;
    [Tooltip("重力の大きさ"), SerializeField] float gravity;

    [Tooltip("重量方向への回転のスピード"), SerializeField] float rotateSpeedTowardGravityUp = 120;

    [Tooltip("重力切り替えの効果を受け取らない時間"), SerializeField] float disableTime = 0.5f;


    /// <summary>
    /// Playerに対する重力源である惑星の向き
    /// </summary>
    Vector3 direction;

    //Rayが接触した惑星のポリゴンの法線
    [Tooltip("接触した惑星の法線"), SerializeField] Vector3 normalVec = Vector3.zero;

    [Header("プレイヤーが重力切り替わりエリアにいるかどうか"), SerializeField] bool isInGravityChangeArea;

    [Header("重力切り替わりエリアにおいて一定時間ごとに重力を変化させる機能を持つコンポーネント"), SerializeField] GravityChanger gravityChanger;

    [Header("重力切り替わりエリアに入った時の上向きベクトルの初期値"), SerializeField] Vector3 up;

    //UniTaskキャンセル
    CancellationTokenSource cancellationTokenSource;
    CancellationToken token;

    /// <summary>
    /// プレイヤーが現在触れたばかりのGravityChanger。新たなGravityChangerに触れるまでその機能を停止するために取得し保存しておく。
    /// 新たに別のGravityChangerに触れたときにその新たなものが登録され、前のGravityChangerへの効果が復活する
    /// </summary>
    [SerializeField] GameObject currentGravityTrap;
    [Tooltip("重力切り替えエリアにおいて重力が反転した時に地面に埋まってしまうのでこの値の分だけ重力方向に移動させる"), SerializeField] 
    float moveDistanceWhenGravityInverse = 0.5f;

    /// <summary>
    /// 時間ごとに重力を変化させる機能のDisposable.
    /// この機能を範囲外に出たときにDispose()できるように保持しておく
    /// </summary>
    IDisposable gravityChangeDisposable;

    /// <summary>
    /// 接触した惑星の法専
    /// </summary>
    public Vector3 NormalVec { get => normalVec; }

    /// <summary>
    /// 重力源となる惑星
    /// </summary>
    public GameObject PlanetObj { get => planetObj; }

    /// <summary>
    /// 小惑星かを判断するための惑星についているコンポーネント
    /// </summary>
    public Planet Planet { get => planet; }

    /// <summary>
    /// 時間ごとに重力を変化させる機能を持つDisposable.
    /// この機能を範囲外に出たときにDispose()できるように変数として取得しておく
    /// </summary>
    public IDisposable GravityChangeDisposable { get => gravityChangeDisposable; }

    private void Awake()
    {
        //UniTaskのキャンセレーショントークンを用意
        cancellationTokenSource = new CancellationTokenSource();
        token = cancellationTokenSource.Token;
        //惑星オブジェクトが登録されていればアタッチされているPlanetコンポーネントを登録
        if (planetObj != null)
        {
            planet = planetObj.GetComponent<Planet>();
        }
        //プレイヤーが死んだとき、ゲームがクリアした時にGravityChangerによる機能を破棄
        GameManager.Instance?.OnPlayerDieObservable.Subscribe(_ => GravityChangeDisposable?.Dispose()).AddTo(this);
        GameManager.Instance?.OnGameClearOrOverObservable.Subscribe(_ => GravityChangeDisposable?.Dispose()).AddTo(this);
    }

    // Start is called before the first frame update
    void Start()
    {

        rb.useGravity = false;

    }

    private void Update()
    {
        Attract();
        PlanetRayCheck();
    }

    /// <summary>
    /// 重力を働かせ、重力方向にオブジェクトを向かせる関数
    /// </summary>

    public void Attract()
    {
        Vector3 gravityUp = normalVec;

        Vector3 bodyUp = transform.up;

        if (rb != null)
        {
            //ゲームポーズ中はあらゆる動きを止めるため重力も働かないようにし、速度を0にする
            if (GameManager.Instance != null && GameManager.Instance.IsPausing && isCharacter)
            {
                if (!rb.isKinematic)
                    rb.velocity = Vector3.zero;
            }
            else
            {
                rb.AddForce(gravityUp * gravity);
            }

        }

        //キャラクターならNormalVecの方向にオブジェクトの上方向が向くようにする
        if (isCharacter)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * transform.rotation;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeedTowardGravityUp * Time.deltaTime);
        }

    }

    /// <summary>
    /// gravityのNormalVecまたはPlanetを登録する関数
    /// </summary>
    /// <param name="setGravity"></param>

    public void SetGravity(Gravity setGravity)
    {
        //登録する側のGravityに惑星が登録されていなければNormalVecを、されていれば惑星を自身に登録する
        if (setGravity.Planet == null)
        {
            normalVec = setGravity.normalVec;
        }

        else
        {
            planetObj = setGravity.PlanetObj;
            planet = setGravity.Planet;
        }

    }

    /// <summary>
    /// Planetを設定する関数。引数で指定したの中からひとつづつ惑星を選び出して、現在重力源としてGravityに登録されている惑星と一致するかどうか審査し、
    /// 一致しなければその惑星を重力の発生源としてGravityに登録する。
    /// </summary>
    /// <param name="setObject"></param>
    /// <param name="planets">惑星の候補</param>
    /// <param name="hasImmovableEffect"></param>
    /// <param name="hasWaitUntilGroundedEffect"></param>
    /// <returns></returns>
    public async UniTask ChoosePlanet(GameObject setObject, List<GameObject> planets, bool hasImmovableEffect, bool hasWaitUntilGroundedEffect)
    {
        try
        {
            //触れたばかりのGravitychangeTrapに再び触れたときは機能しないようにする
            if (currentGravityTrap == setObject) return;
            //GravityChangeTrap登録
            currentGravityTrap = setObject;

            for (int i = 0; i < planets.Count; i++)
            {
                //候補の中から選ばれた惑星が現在自身のGravityに登録されている惑星と一致する場合は審査を継続
                if (planets[i] == planetObj) continue;
                //一致しなければその候補の惑星を重力源として登録してループを抜ける。
                else
                {
                    planetObj = planets[i];
                    planet = planetObj.GetComponent<Planet>();
                    break;
                }
            }

            //着地するまでプレイヤーが動かないようにする
            if (hasImmovableEffect)
            {
                movable.GoToImmovable();
            }

            //惑星登録後しばらく無効化しておかないと複数回この関数が呼ばれて一瞬で惑星が切り替わるというバグが起こるので
            //一定時間無効化する。
            await UniTask.Delay(System.TimeSpan.FromSeconds(disableTime), cancellationToken: token);
            if (movable != null && hasImmovableEffect)
            {
                if (hasWaitUntilGroundedEffect)
                {
                    await UniTask.WaitUntil(() => movable.IsGround);
                }
                movable.GoToMovable();
            }
            //GravityChangeTrap
            currentGravityTrap = null;
        }

        catch (OperationCanceledException)
        {
            DebugLog.Log("途中で中断しました");
        }
    }

    /// <summary>
    /// 重力源である惑星の方向にレイを飛ばしてNormalVecを設定する関数。惑星上にいない場合は機能しない。
    /// </summary>
    public void PlanetRayCheck()
    {
        if (planetObj == null) return;
        //惑星の方向にレイを飛ばし、感知した法線ベクトルをNormalVecとして登録。
        direction = (planetObj.transform.position - transform.position).normalized;

        RaycastHit hit;
        Ray ray = new Ray(transform.position, direction);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Planet"))
            {
                normalVec = hit.normal;
            }

        }

    }

    /// <summary>
    /// planetがない状況で重力を設定する関数。
    /// </summary>
    /// <param name="setObject"></param>
    /// <param name="hasImmovableEffect"></param>
    /// <param name="hasWaitUntilGroundedEffect"></param>
    /// <param name="direction"></param>
    /// <returns></returns>

    public async UniTask SetGravity(GameObject setObject, bool hasImmovableEffect, bool hasWaitUntilGroundedEffect, Vector3 direction)
    {
        try
        {
            //触れたばかりのGravityChangeTrapに再び触れたときには機能しないようにする
            if (setObject == currentGravityTrap) return;
            //GravityChangeTrap登録
            currentGravityTrap = setObject;
            //惑星に従った重力ではないので重力源の惑星は空にする
            planetObj = null;
            planet = null;

            //NormalVecを引数に指定したものに設定
            normalVec = direction;
            //重力を切り替える際に移動を止める設定になっている場合には移動を止める
            if (hasImmovableEffect)
            {
                movable?.GoToImmovable();
            }
            await UniTask.Delay(System.TimeSpan.FromSeconds(disableTime), cancellationToken: token);
            if (movable != null)
            {
                //重力切り替え装置に触れた後空中にいる間に別の重力切り替え装置に触れる可能性がある場合、着地まで重力が変わらないと困るので
                //着地まで待つか待たないかを決めるフラグを用意する。
                if (hasWaitUntilGroundedEffect)
                {
                    await UniTask.WaitUntil(() => movable.IsGround);

                }


                if (hasImmovableEffect)
                {
                    movable?.GoToMovable();
                }
            }

            currentGravityTrap = null;
        }

        catch (OperationCanceledException)
        {
            DebugLog.Log("中断されました");
        }
    }

    /// <summary>
    /// 一定時間ごとに重力が切り替わるようにする関数
    /// </summary>
    /// <param name="setChanger"></param>
    public void SetGravityChanger(GravityChanger setChanger)
    {
        //GravityChanger登録
        gravityChanger = setChanger;
        //自身の上方向を基準となる上方向として登録
        up = transform.up;

        //基準となる上方向ベクトルにGravityChangerのGravityDirection(一定時間ごとに1 / -1に切り替わるUniRxのReactiveProperty)
        //をかけて一定時間ごとに重力が切り替わるようにする。
        gravityChangeDisposable = gravityChanger?.GravityDirection.Subscribe(direction =>
        {
            normalVec = up * direction;
            transform.position += -normalVec * moveDistanceWhenGravityInverse;
            transform.rotation = Quaternion.Euler(0f, 180, 0f) * transform.rotation;
        }).AddTo(this);

    }

    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
    }
}
