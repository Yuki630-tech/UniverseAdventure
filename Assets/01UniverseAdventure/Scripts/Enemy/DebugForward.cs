using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniRx;
#if UNITY_EDITOR
[ExecuteInEditMode]
/// <summary>
/// 鉄球が転がっていく方向をわかりやすくするためのクラス
/// </summary>
public class DebugForward : MonoBehaviour
{
    /// <summary>
    /// debug点の惑星表面からの距離
    /// </summary>
    Vector3 offset;
    [Tooltip("ギズモの頂点数"), SerializeField] int vertexNum = 100;
    [Tooltip("ギズモ間の距離"), SerializeField] float vertexInterval = 3f;
    /// <summary>
    /// 更新前のオブジェクトの座標
    /// </summary>
    Vector3 lastPosition;
    [Tooltip("回転した際に登録する回転前のローテーション"), SerializeField] Quaternion lastRotation;

    /// <summary>
    /// debug点のリスト
    /// </summary>
    List<Vector3> debugVertexSet = new List<Vector3>();
    [Tooltip("ギズモを添わせる対象のオブジェクト"),SerializeField] GameObject planet;

    private void Awake()
    {
        StartCoroutine(Sort());

    }
    private void OnValidate()
    {
        
        //座標と回転情報をキャッシュ
        lastPosition = transform.position;　
        lastRotation = transform.rotation;

        //undoしたときにrayが消えるようにする
        Undo.undoRedoPerformed += OnUndoRedo;
        
       
    }
    /// <summary>
    /// rayを地面に沿わせるようにする関数
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    Vector3 CheckGroundOffset(Vector3 origin)
    {
        //指定の座標からgravityクラスに登録されているplanetオブジェクトに向かってRayを飛ばす
        var direction = (planet.transform.position - origin).normalized;
        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit))
        {
            //ヒットしたオブジェクトがgravityに登録されているplanetオブジェクトならヒットした場所までの距離を返す
            if (hit.collider.gameObject == planet)
            {
                return direction * hit.distance;
            }

    　　//それ以外ならoffsetの値をそのまま返す
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

    private void OnDrawGizmos()
    {
        //再生モードか、Planetオブジェクトがない場合はギズモを描かない
        if (Application.isPlaying || planet == null) return;
        //敵を移動、回転させた時にギズモが不自然にならないように回転前に存在したギズモの頂点を削除する
        if (transform.rotation != lastRotation)
        {

            debugVertexSet.Clear();
            lastRotation = transform.rotation;

        }

        if (transform.position != lastPosition)
        {
            debugVertexSet.Clear();
            lastPosition = transform.position;

        }
        Gizmos.color = Color.red;

        //惑星に沿ったレイのギズモを描く
        for (int i = 0; i < debugVertexSet.Count - 3; i++)
        {
            var direction = debugVertexSet[i + 1] - debugVertexSet[i];
            direction = direction.normalized;
            var distance = Vector3.Distance(debugVertexSet[i], debugVertexSet[i + 1]);
            Gizmos.DrawRay(debugVertexSet[i], direction * distance);
        }
    }

    /// <summary>
    /// UnDoしたときにポイントを削除してギズモが重ならないようにする
    /// </summary>
    void OnUndoRedo()
    {
        if (gameObject != null && Selection.Contains(gameObject))
        {
            debugVertexSet.Clear();
            SetPoints();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator Sort()
    {
        if(Application.isPlaying) yield break;
        while (true)
        {
            SetPoints();
            yield return new WaitUntil(() => transform.rotation != lastRotation || transform.position != lastPosition);
        }
    }

    void SetPoints()
    {
        for (int i = 0; i < vertexNum; i++)
        {
            var pos = transform.forward * i * vertexInterval;

            var worldPos = transform.position + pos;
            offset = CheckGroundOffset(worldPos);
            var correction = (worldPos - planet.transform.position).normalized;
            worldPos = worldPos + offset + correction;
            debugVertexSet.Add(worldPos);

        }

        debugVertexSet.Sort((a, b) =>
        {
            float distA = Vector3.Distance(a, transform.position);
            float distB = Vector3.Distance(b, transform.position);
            return distA.CompareTo(distB);
        });
    }


}
#endif