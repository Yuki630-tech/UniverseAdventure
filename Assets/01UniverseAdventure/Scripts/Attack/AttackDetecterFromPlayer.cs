using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackDetecterFromPlayer : MonoBehaviour
{
    [Tooltip("プレイヤーが攻撃した際にプレイヤー側に対して行うイベント"), SerializeField] UnityEvent unityEvent;
    private void OnTriggerEnter(Collider other)
    {
        var attackReceiverFromPlayer = other.GetComponent<AttackReceiverFromPlayer>();

        if (attackReceiverFromPlayer != null)
        {
            //レシーバー側にダメージを与える
            attackReceiverFromPlayer.OnReceivedDamageFromPlayer();

            //登録されたイベントを実行
            unityEvent?.Invoke();
        }
    }
}
