using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerCollider : MonoBehaviour
{
    [Tooltip("���_�̐�"), SerializeField] int numberOfCol;
    [Tooltip("�~��`���Ă���LineRenderer"), SerializeField] LineRenderer lineRenderer;
    [Tooltip("�v���C���[�ւ̃_���[�W��^���邽�߂̃f�[�^"), SerializeField] AttackData data;
    public void SetColNumber(int setNum)
    {
        numberOfCol = setNum;
    }

    private void Update()
    {
        LazerColliderTask();


    }

    /// <summary>
    /// ���[�U�[�ɉ������R���C�_�[��t���鏈��
    /// </summary>

    void LazerColliderTask()
    {
        RaycastHit hit;

        //��̒��_���炻�ׂ̗̒��_�Ƀ��C���΂��Ƃ�����Ƃ����[�U�[�̒��_�̐������J��Ԃ�
        for (int i = 0; i < numberOfCol; i++)
        {
            //�R���C�_�[�Ɏg�p����n�_�ƏI�_�̍��W���擾�B
            //GetPosition()��lineRenderer�̃I�u�W�F�N�g����̑��ΓI�Ȉʒu�Ȃ̂�lineRenderer�I�u�W�F�N�g�̍��W�ɑ����΃��[���h���W�����߂���
            var directionOfStart = lineRenderer.GetPosition(i); 
            directionOfStart = transform.rotation * directionOfStart; //���[�U�[���ˑ䂪��]���Ă���Ƃ��̂��߂ɕ����x�N�g����lineRenderer��rotation����]�����Ă���
            var start = transform.position + directionOfStart;
            var directionOfEnd = lineRenderer.GetPosition(i + 1); //�ׂ̒��_�������悤�ɋ��߂�
            directionOfEnd = transform.rotation * directionOfEnd;
            var end = transform.position + directionOfEnd;

            var distance = Vector3.Distance(start, end);
            //start����end�Ɍ����ă��C���΂��ăv���C���[�ɓ���������_���[�W��^����
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
