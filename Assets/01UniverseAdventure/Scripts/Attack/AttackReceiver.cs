using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackReceiver : MonoBehaviour
{
    [Tooltip("ダメージを受けた際のイベントを登録する場所"),SerializeField] UnityEvent<AttackData> unityEvent;

    /// <summary>
    /// ダメージを受けたときのイベントを発生させる関数
    /// </summary>
    /// <param name="attackData"></param>
    public void OnReceivedDamage(AttackData attackData)
    {
        unityEvent.Invoke(attackData);
    }
}
