using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerCollider : MonoBehaviour
{
    [Tooltip("頂点の数"), SerializeField] int numberOfCol;
    [Tooltip("円を描いているLineRenderer"), SerializeField] LineRenderer lineRenderer;
    [Tooltip("プレイヤーへのダメージを与えるためのデータ"), SerializeField] AttackData data;
    public void SetColNumber(int setNum)
    {
        numberOfCol = setNum;
    }

    private void Update()
    {
        LazerColliderTask();


    }

    /// <summary>
    /// レーザーに沿ったコライダーを付ける処理
    /// </summary>

    void LazerColliderTask()
    {
        RaycastHit hit;

        //一つの頂点からその隣の頂点にレイを飛ばすという作業をレーザーの頂点の数だけ繰り返す
        for (int i = 0; i < numberOfCol; i++)
        {
            //コライダーに使用する始点と終点の座標を取得。
            //GetPosition()はlineRendererのオブジェクトからの相対的な位置なのでlineRendererオブジェクトの座標に足せばワールド座標を求められる
            var directionOfStart = lineRenderer.GetPosition(i); 
            directionOfStart = transform.rotation * directionOfStart; //レーザー発射台が回転しているときのために方向ベクトルをlineRendererのrotation分回転させておく
            var start = transform.position + directionOfStart;
            var directionOfEnd = lineRenderer.GetPosition(i + 1); //隣の頂点も同じように求める
            directionOfEnd = transform.rotation * directionOfEnd;
            var end = transform.position + directionOfEnd;

            var distance = Vector3.Distance(start, end);
            //startからendに向けてレイを飛ばしてプレイヤーに当たったらダメージを与える
            var direction = (end - start).normalized;
            if (Physics.Raycast(start, direction, out hit, distance))
            {
                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    var attackReceiver = hit.collider.gameObject.GetComponent<AttackReceiver>();
                    attackReceiver.OnReceivedDamage(data);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < numberOfCol; i++)
        {
            var directionOfStart = lineRenderer.GetPosition(i);
            directionOfStart = transform.rotation * directionOfStart;
            var start = transform.position + directionOfStart;
            var directionOfEnd = lineRenderer.GetPosition(i + 1);
            directionOfEnd = transform.rotation * directionOfEnd;
            var end = transform.position + directionOfEnd;

            var distance = Vector3.Distance(start, end);
            var direction = (end - start).normalized;
            Gizmos.DrawRay(start, direction * distance);
        }
    }
}
