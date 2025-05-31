using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �~�`�̃��[�U�[�𔭐�������I�u�W�F�N�g
/// </summary>
public class LazerSpawner : MonoBehaviour
{
    [Tooltip("���������郌�[�U�[�̃v���n�u"), SerializeField] CircleLazer lazerPrefab;
    [Tooltip("1�E�F�[�u�ɔ���������~�̐�"),SerializeField] int spawnNumverPerWave;
    [Tooltip("���[�U�[�𔭐�������^�C�~���O�����炷���ǂ���"), SerializeField] bool isDelay;
    [Tooltip("�~�𔭐�������Ԋu"), SerializeField] float spawnInterval = 1f;
    [Tooltip("�E�F�[�u�Ԃ̊Ԋu"), SerializeField] float waveInterval = 3f;
    [Tooltip("���[�U���g�傳����ő�̔��a"), SerializeField] float maxRadius;
    [Tooltip("���[�U�[�����킹��Ώۂ̘f��"), SerializeField] GameObject planet;
    [Tooltip("���[�U�����ꂷ�����Ƃ݂Ȃ�����"), SerializeField] float limitedDistance;
    [Tooltip("���[�U�[����������y�����̋���"), SerializeField] float destroyDistanceY;

    [Tooltip("���[�U�[�����킹��f��"), SerializeField] GameObject planetObj;

    /// <summary>
    /// ���[�U�[���ˑ����������Ɨ������邽�߂ɘf���̖@�����擾���郌�C���΂�����
    /// </summary>
    Vector3 direction;


    private void Awake()
    {
        //planet���o�^����Ă��Ȃ��ꍇ�͉������Ȃ�
        if (planetObj == null) return;
        //�f���̕����Ƀ��C���΂��Ė@�����擾���A���̌����ɔ��ˑ�̏�����������悤�ɂ���B
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
        //���[�U�[�𔭎�
        StartCoroutine(SpawnLazer());
    }

    /// <summary>
    /// ���[�U�[���˂̃R���[�`��
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
                lazer.SetLazerSetting(maxRadius, planet, limitedDistance, destroyDistanceY); //�����������~�`���[�U�[�ɂ��čL����ő唼�a�Ɠ��������킹��Ώۂ�planet��o�^
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
