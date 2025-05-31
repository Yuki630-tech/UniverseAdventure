using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CustomGravityChanger : MonoBehaviour
{
    [Tooltip("触れたときにプレイヤーを動かないようにするかどうか"), SerializeField] bool hasImmovableEffect;
    [Tooltip("プレイヤーが接地するまで次の重力切り替えが起こらないようにするかどうか"), SerializeField] bool hasWaitUntilGroundedEffect;

    /// <summary>
    /// 機能が有効かどうか
    /// </summary>
    bool isEffective;
    private void Awake()
    {
        isEffective = true;
        //プレイヤーが死亡してリスタートした時再度機能を有効にする
        GameManager.Instance?.OnPlayerRestartObservable.Where(_ => !isEffective).Subscribe(_ => isEffective = true).AddTo(gameObject);
    }

    private async void OnTriggerEnter(Collider other)
    {
        var gravity = other.GetComponent<Gravity>();
        if(gravity != null && isEffective)
        {
            
            //自身の上方向を触れた相手の重力の上方向に設定する。
            await gravity.SetGravity(gameObject, hasImmovableEffect, hasWaitUntilGroundedEffect, transform.up);
            isEffective = false;
        }
    }
}
