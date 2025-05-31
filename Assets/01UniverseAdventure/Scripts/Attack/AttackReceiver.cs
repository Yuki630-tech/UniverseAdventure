using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackReceiver : MonoBehaviour
{
    [Tooltip("�_���[�W���󂯂��ۂ̃C�x���g��o�^����ꏊ"),SerializeField] UnityEvent<AttackData> unityEvent;

    /// <summary>
    /// �_���[�W���󂯂��Ƃ��̃C�x���g�𔭐�������֐�
    /// </summary>
    /// <param name="attackData"></param>
    public void OnReceivedDamage(AttackData attackData)
    {
        unityEvent.Invoke(attackData);
    }
}
