using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackReceiverFromPlayer : MonoBehaviour
{
    [Tooltip("�v���C���[����U�����󂯂��Ƃ��̃C�x���g"), SerializeField] UnityEvent unityEvent;
    public void OnReceivedDamageFromPlayer()
    {
        unityEvent.Invoke();
    }
}
