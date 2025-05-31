using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class TekkyuEnemy : MonoBehaviour
{
    [Tooltip("“G‚ª”­Ë‚·‚é“S‹…‚ÌƒvƒŒƒnƒu"),SerializeField] GameObject tekkyuPrefab;
    [Tooltip("“S‹…‚ª”­Ë‚³‚ê‚éˆÊ’u"),SerializeField] Transform spawner;
    [Tooltip("©g‚ÌGravity"),SerializeField] Gravity gravity;
    [Tooltip("“S‹…‚ª”­Ë‚³‚ê‚éŠÔŠu"), SerializeField] float spawnInterval = 2f;
    [Tooltip("“S‹…‚É“­‚©‚¹‚é—Í‚Ì‹­‚³"), SerializeField] float power = 2f;

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

            //“S‹…‚ÌGravity‚ğæ“¾B
            var tekkyuGravity = tekkyuObject.GetComponent<Gravity>();
            //“S‹…‚ÌGravity‚ÌNormalVec‚Ü‚½‚ÍPlanet‚É©g‚Ì‚à‚Ì‚ğ“o˜^‚µAd—Í‚Ì•ûŒü‚Æ‰ˆ‚í‚¹‚é‘ÎÛ‚ÌPlanet‚ğİ’è‚·‚é
            tekkyuGravity?.SetGravity(gravity);

            //“S‹…‚ğ‘O•ûŒü‚É”ò‚Î‚·
            var tekkyuRb = tekkyuObject.GetComponent<Rigidbody>();
            tekkyuRb.AddForce(transform.forward * power, ForceMode.Impulse);

            yield return new WaitForSeconds(spawnInterval);
        }
    }


}
