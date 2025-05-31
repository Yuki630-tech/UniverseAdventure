using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UniRx;
using System.ComponentModel;

public class AudioManager : MonoBehaviour
{
    [Tooltip("ゲーム音楽を鳴らすオーディオソース"), SerializeField] AudioSource audioSource;
    [Tooltip("SEを鳴らすオーディオソース"), SerializeField] AudioSource seAjudioSource;
    [Tooltip("ゲームの音楽、SEとそれらの名前をまとめたデータ"), SerializeField] AudioClipData audioClipData;
    [Tooltip("ポーズ中のオーディオの音量"), SerializeField] float volumeWhilePause;
    [Header("死亡した後にチェックポイントに戻ってきたときに鳴らすオーディオクリップ"), SerializeField, ] AudioClip clipBeforeDie;
    [Header("Pauseする前の音量"), SerializeField] float volumeBeforePause;
    /// <summary>
    /// オーディオマネージャーのインスタンス
    /// </summary>
    public static AudioManager Instance {  get; private set; }

    /// <summary>
    /// ポーズ中のオーディオの音量
    /// </summary>
    public float VolumeWhilePause { get => volumeWhilePause; }

    private void Awake()
    {
        //シングルトン
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }

        //プレイヤーが死亡してゲームがリスタートしたときにbgmをチェックポイントにいたときのものに変更する。
        GameManager.Instance?.OnPlayerRestartObservable.Subscribe(_ => InitializeAudio()).AddTo(this);


    }

    /// <summary>
    /// AudioSourceが鳴り始めたことを感知する関数
    /// </summary>
    /// <returns></returns>

    public bool IsAudioPlaying()
    {
        return audioSource.isPlaying;
    }

    /// <summary>
    /// bgmを死亡前チェックポイントを通った時に流れていたbgmに変更する関数
    /// </summary>

    void InitializeAudio()
    {
        //オーディオソースのクリップをチェックポイントに触れたときに流れていたものに設定して再生する
        audioSource.clip = clipBeforeDie;
        audioSource.loop = true;
        audioSource.Play();
    }

    /// <summary>
    /// ゲームをリスタートさせたときに流れるbgmを登録する関数
    /// </summary>

    public void SetAudioBeforeDie()
    {
        //現在設定されているものをリスタート時に流すクリップとして登録
        clipBeforeDie = audioSource.clip;
    }

    /// <summary>
    /// AudioSourceをfadeoutさせる関数
    /// </summary>
    /// <param name="fadeOutTime"></param>

    public void FadeOut(float fadeOutTime)
    {
        audioSource.DOFade(0, fadeOutTime);
    }

    /// <summary>
    /// AudioSourceを止める関数
    /// </summary>

    public void StopAudio()
    {
        audioSource.Pause();
    }

    /// <summary>
    /// AudioClipを指定して再生する関数
    /// </summary>
    /// <param name="clipName"></param>

    public void PlayAudio(AudioClipData.AudioClipName clipName)
    {

        audioSource.volume = 1f;
        //AudioClipのデータから指定したclipnameに一致したbgmdataを取得する
        var data = audioClipData.BgmDatas.FirstOrDefault(data => data.Name == clipName);
        audioSource.clip = data.AudioClip; //取得したデータのAudioClipをAudioSourceに登録する

        //死亡した時とGameOverした時のbgmは何度も流すと絶望感が増してさすがにかわいそうなので1度だけ流すようにする
        if (clipName == AudioClipData.AudioClipName.DieBGM || clipName == AudioClipData.AudioClipName.GameOverBGM)
        {
            audioSource.loop = false;
        }

        //そのほかのbgmはループするようにする
        else
        {
            audioSource.loop = true;
        }
        audioSource.Play();
    }

    /// <summary>
    /// SEを指定して鳴らす処理
    /// </summary>
    /// <param name="clipName"></param>
    public void PlaySE(AudioClipData.SeAudioClipName clipName)
    {
        var se = audioClipData.SeDatas.Find(data => data.Name == clipName).AudioClip;
        seAjudioSource.PlayOneShot(se);
    }

    /// <summary>
    /// ポーズした時に音量が下がるようにする
    /// </summary>
    public void ChangeAudioToPauseMode()
    {
        volumeBeforePause = audioSource.volume;
        audioSource.volume = volumeWhilePause;
    }

    /// <summary>
    /// ポーズ解除した時に音量を戻す
    /// </summary>

    public void ChangeAudioToUnPauseMode()
    {
        audioSource.volume = volumeBeforePause;
    }


}
