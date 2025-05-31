using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class TekkyuEnemy : MonoBehaviour
{
    [Tooltip("�G�����˂���S���̃v���n�u"),SerializeField] GameObject tekkyuPrefab;
    [Tooltip("�S�������˂����ʒu"),SerializeField] Transform spawner;
    [Tooltip("���g��Gravity"),SerializeField] Gravity gravity;
    [Tooltip("�S�������˂����Ԋu"), SerializeField] float spawnInterval = 2f;
    [Tooltip("�S���ɓ�������͂̋���"), SerializeField] float power = 2f;

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

            //�S����Gravity���擾�B
            var tekkyuGravity = tekkyuObject.GetComponent<Gravity>();
            //�S����Gravity��NormalVec�܂���Planet�Ɏ��g�̂��̂�o�^���A�d�͂̕����Ɖ��킹��Ώۂ�Planet��ݒ肷��
            tekkyuGravity?.SetGravity(gravity);

            //�S����O�����ɔ�΂�
            var tekkyuRb = tekkyuObject.GetComponent<Rigidbody>();
            tekkyuRb.AddForce(transform.forward * power, ForceMode.Impulse);

            yield return new WaitForSeconds(spawnInterval);
        }
    }


}
