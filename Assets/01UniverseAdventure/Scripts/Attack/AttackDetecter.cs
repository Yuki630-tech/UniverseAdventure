using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDetecter : MonoBehaviour
{
    [Tooltip("�U���͂ƃm�b�N�o�b�N�x�N�g���̃f�[�^"), SerializeField] AttackData data;

   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack")) return;
        var attackReceiver = other.GetComponent<AttackReceiver>();

        if(attackReceiver != null)
        {
            //���V�[�o�[���Ƀ_���[�W��^����
            attackReceiver.OnReceivedDamage(data);
        }
    }
}
