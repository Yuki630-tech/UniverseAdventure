using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [Tooltip("���C�̎n�_"), SerializeField] float center;
    [Tooltip("���C�̐�[�̋��̔��a"), SerializeField] float radius;
    [Tooltip("���C�̒���"), SerializeField] float length;
    [Tooltip("�������g�������Ă���Gravity�R���|�[�l���g"), SerializeField] Gravity gravity;


    [Header("�ڒn���Ă��邩�ǂ���"), SerializeField] bool isGround;

    [Header("�ڒn�m�F���s���Ă��邩�ǂ���"), SerializeField] bool isGroundCheckEffective = true;


    RaycastHit hit;

    public bool IsGround { get => isGround; }

    private void Start()
    {
        
    }

    public void UpdateGroundCheck()
    {
        //�W�����v���������΂炭�O�����h�`�F�b�N���Ȃ��悤�ɂ���(�W�����v�����u�Ԃ͒n�ʂɂ��Ă��Đڒn����ɂȂ��Ă��܂�����)
        if (isGroundCheckEffective)
        {
            //���C�̎n�_���v���C���[�̍��W(����)����gravity��NormalVec�̕�����center�������ړ������ꏊ�ɐݒ肷�遨�v���C���[���f���ɉ����ĕ������ۂɂ�
            //���C���K�؂ɔ�Ԃ悤�ɁB�n�_����NormalVec�Ƃ͋t����(�v���C���[�̑����Ɍ�������)���C���΂��ڒn���������B
            var origin = transform.position + gravity.NormalVec * center;
            if (Physics.SphereCast(origin, radius, -gravity.NormalVec, out hit, length))
            {
                
                if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Planet"))
                {
                    isGround = true;
                }
               
            }

            else
            {
                isGround = false;
            }
        }

        else
        {
            isGround = false;
        }
        
    }
    /// <summary>
    /// �W�����v����ɐڒn��������Ȃ��悤�ɂ��΂炭�ڒn������s��Ȃ��悤�ɂ���֐��B
    /// </summary>
    public async void DisableToCheckGround()
    {
        isGroundCheckEffective = false;
        await UniTask.Delay(1000);
        isGroundCheckEffective = true;
    }

    private void OnDrawGizmos()
    {
        var origin = transform.position + center * gravity.NormalVec;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(origin, -gravity.NormalVec * length);

        Gizmos.DrawWireSphere(origin - transform.up * length, radius);
    }
}
