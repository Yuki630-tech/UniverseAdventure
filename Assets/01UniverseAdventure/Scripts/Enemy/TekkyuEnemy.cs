using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class TekkyuEnemy : MonoBehaviour
{
    [Tooltip("敵が発射する鉄球のプレハブ"),SerializeField] GameObject tekkyuPrefab;
    [Tooltip("鉄球が発射される位置"),SerializeField] Transform spawner;
    [Tooltip("自身のGravity"),SerializeField] Gravity gravity;
    [Tooltip("鉄球が発射される間隔"), SerializeField] float spawnInterval = 2f;
    [Tooltip("鉄球に働かせる力の強さ"), SerializeField] float power = 2f;

    [SerializeField] bool isDelay;
    [SerializeField] float delayRate = 2;
    [SerializeField] Coroutine spawnCoroutine;
    
    //Start is called before the first frame update
    void Start()
    {
        spawnCoroutine = StartCoroutine(SpawnTekkyu());
    }

    IEnumerator SpawnTekkyu()
    {
        if (isDelay)
        {
            yield return new WaitForSeconds(spawnInterval / delayRate);
        }
        while (true)
        {
            var tekkyuObject = Instantiate(tekkyuPrefab, spawner.transform.position, transform.rotation);

            //鉄球のGravityを取得。
            var tekkyuGravity = tekkyuObject.GetComponent<Gravity>();
            //鉄球のGravityのNormalVecまたはPlanetに自身のものを登録し、重力の方向と沿わせる対象のPlanetを設定する
            tekkyuGravity?.SetGravity(gravity);

            //鉄球を前方向に飛ばす
            var tekkyuRb = tekkyuObject.GetComponent<Rigidbody>();
            tekkyuRb.AddForce(transform.forward * power, ForceMode.Impulse);

            yield return new WaitForSeconds(spawnInterval);
        }
    }


}
