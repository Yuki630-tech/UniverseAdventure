using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class AudioPlayObj : MonoBehaviour
{
    [Tooltip("プレイヤーが通過した時に鳴らすAudioClipの名前"), SerializeField] AudioClipData.AudioClipName clipName;
    [Header("プレイヤーが通過した時にAudioを切り替えて再生するかどうか"), SerializeField] bool isEffective;

    private void Awake()
    {
        isEffective = true;
        //プレイヤーがリスタートした際にこのオブジェクトを有効にしてプレイヤーが触れたときにaudioが切り替わるようにする
        GameManager.Instance?.OnPlayerRestartObservable.Where(_ => !isEffective).Subscribe(_ => isEffective = true).AddTo(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!isEffective) { return; }
        if (other.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance?.PlayAudio(clipName);
            //プレイヤーが戻ってきて再度触れたときにaudioが不自然に切り替わらないように機能を無効化しておく
            isEffective = false;

        }
    }
}
