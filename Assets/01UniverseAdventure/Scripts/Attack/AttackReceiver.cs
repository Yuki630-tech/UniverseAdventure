using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackReceiver : MonoBehaviour
{
    [Tooltip("ダメージを受けた際のイベントを登録する場所"),SerializeField] UnityEvent<AttackData> unityEvent;
    [Tooltip("攻撃判定のタイプ→このタイプと同じ攻撃判定にあたった時にイベントを発生させる"), SerializeField] private AttackDetecter.DetecterType detecterType;
    [Header("有効かどうか"), SerializeField] private bool isEnable;

    private void OnEnable()
    {
        isEnable = true;
    }
    public AttackDetecter.DetecterType DetecterType { get => detecterType; }

    /// <summary>
    /// ダメージを受けたときのイベントを発生させる関数
    /// </summary>
    /// <param name="attackData"></param>
    public void OnReceivedDamage(AttackData attackData)
    {
        if(!isEnable) return;
        //DebugLog.Log($"{DetecterType} 「いってえな」");
        unityEvent.Invoke(attackData);
        if(detecterType != AttackDetecter.DetecterType.Enemy)
        {
            isEnable = false;
        }
        
        
    }
}
