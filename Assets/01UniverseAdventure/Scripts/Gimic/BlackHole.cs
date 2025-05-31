using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System;

public class BlackHole : MonoBehaviour
{
    bool isEffective; //プレイヤーが触れたことを感知するかどうか
    [SerializeField] AudioSource audioSource;

    private void Awake()
    {
        isEffective = true;
        //リスタートした時に機能を有効にする
        GameManager.Instance?.OnPlayerRestartObservable.Where(_ => !isEffective).Subscribe(_ => isEffective = true).AddTo(this);
        //プレイヤーが死んだとき、新たなブラックホールに再度吸い込まれてまた死亡処理を行うことがないように機能を停止しておく
        GameManager.Instance?.OnPlayerDieObservable.Subscribe(_ => isEffective = false).AddTo(this);
        GameManager.Instance?.OnGameClearOrOverObservable.Subscribe(_ => isEffective = false).AddTo(this);

        //ポーズした時に音量を落とし、ポーズ解除した時に音量を戻す。
        GameManager.Instance?.OnPauseGameObservable.Where(_ => AudioManager.Instance != null).Subscribe(_ => audioSource.volume = AudioManager.Instance.VolumeWhilePause).AddTo(this);
        GameManager.Instance?.OnUnPauseGameObservable.Subscribe(_ => audioSource.volume = 1f).AddTo(this);
    }

    /// <summary>
    /// プレイヤーがブラックホールに吸い込まれる範囲内に入った時に行う処理
    /// </summary>
    /// <param name="other"></param>
    public void OnPlayerTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();

        if(player != null && isEffective)
        {
            //ブラックホールに吸い込まれる挙動を開始する
            player.ChangeToBlackHoleState(transform);
            
        }
    }

    /// <summary>
    /// プレイヤーがデッドゾーンに入った時に行う処理
    /// </summary>
    /// <param name="other"></param>
    public void OnPlayerEnterToDeadZone(Collider other)
    {
        var player = other.GetComponent<Player>();

        if(player != null && isEffective)
        {
            player.OnDie();
        }
    }
}
