using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackReceiverFromPlayer : MonoBehaviour
{
    [Tooltip("プレイヤーから攻撃を受けたときのイベント"), SerializeField] UnityEvent unityEvent;
    public void OnReceivedDamageFromPlayer()
    {
        unityEvent.Invoke();
    }
}
