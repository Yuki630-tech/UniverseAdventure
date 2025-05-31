using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 円形のレーザーを発生させるオブジェクト
/// </summary>
public class LazerSpawner : MonoBehaviour
{
    [Tooltip("発生させるレーザーのプレハブ"), SerializeField] CircleLazer lazerPrefab;
    [Tooltip("1ウェーブに発生させる円の数"),SerializeField] int spawnNumverPerWave;
    [Tooltip("レーザーを発生させるタイミングをずらすかどうか"), SerializeField] bool isDelay;
    [Tooltip("円を発生させる間隔"), SerializeField] float spawnInterval = 1f;
    [Tooltip("ウェーブ間の間隔"), SerializeField] float waveInterval = 3f;
    [Tooltip("レーザを拡大させる最大の半径"), SerializeField] float maxRadius;
    [Tooltip("レーザーを沿わせる対象の惑星"), SerializeField] GameObject planet;
    [Tooltip("レーザが離れすぎだとみなす距離"), SerializeField] float limitedDistance;
    [Tooltip("レーザーを消去するy方向の距離"), SerializeField] float destroyDistanceY;

    [Tooltip("レーザーを沿わせる惑星"), SerializeField] GameObject planetObj;

    /// <summary>
    /// レーザー発射台をしっかりと立たせるために惑星の法線を取得するレイを飛ばす方向
    /// </summary>
    Vector3 direction;


    private void Awake()
    {
        //planetが登録されていない場合は何もしない
        if (planetObj == null) return;
        //惑星の方向にレイを飛ばして法線を取得し、その向きに発射台の上向きが向くようにする。
        direction = (planetObj.transform.position - transform.position).normalized;

        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;
        Vector3 normalVec;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Planet"))
            {
                normalVec = hit.normal;
                var up = Quaternion.FromToRotation(transform.up, normalVec) * transform.rotation;
                transform.rotation = up;
            }

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //レーザーを発射
        StartCoroutine(SpawnLazer());
    }

    /// <summary>
    /// レーザー発射のコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnLazer()
    {
        if (isDelay)
        {
            yield return new WaitForSeconds(spawnInterval * spawnNumverPerWave);
        }
        while (true)
        {
            for(int i = 0; i < spawnNumverPerWave; i++)
            {
                var lazer = Instantiate(lazerPrefab, transform.position, transform.rotation);
                lazer.SetLazerSetting(maxRadius, planet, limitedDistance, destroyDistanceY); //発生させた円形レーザーについて広がる最大半径と動きを沿わせる対象のplanetを登録
                yield return new WaitForSeconds(spawnInterval);
            }
            yield return new WaitForSeconds(waveInterval);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gameObject.transform.position, maxRadius);
    }
}
