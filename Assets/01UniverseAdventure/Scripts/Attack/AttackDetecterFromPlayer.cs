using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackDetecterFromPlayer : MonoBehaviour
{
    [Tooltip("�v���C���[���U�������ۂɃv���C���[���ɑ΂��čs���C�x���g"), SerializeField] UnityEvent unityEvent;
    private void OnTriggerEnter(Collider other)
    {
        var attackReceiverFromPlayer = other.GetComponent<AttackReceiverFromPlayer>();

        if (attackReceiverFromPlayer != null)
        {
            //���V�[�o�[���Ƀ_���[�W��^����
            attackReceiverFromPlayer.OnReceivedDamageFromPlayer();

            //�o�^���ꂽ�C�x���g�����s
            unityEvent?.Invoke();
        }
    }
}
