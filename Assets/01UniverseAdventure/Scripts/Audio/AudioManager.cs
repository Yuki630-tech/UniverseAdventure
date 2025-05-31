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
    [Tooltip("�Q�[�����y��炷�I�[�f�B�I�\�[�X"), SerializeField] AudioSource audioSource;
    [Tooltip("SE��炷�I�[�f�B�I�\�[�X"), SerializeField] AudioSource seAjudioSource;
    [Tooltip("�Q�[���̉��y�ASE�Ƃ����̖��O���܂Ƃ߂��f�[�^"), SerializeField] AudioClipData audioClipData;
    [Tooltip("�|�[�Y���̃I�[�f�B�I�̉���"), SerializeField] float volumeWhilePause;
    [Header("���S������Ƀ`�F�b�N�|�C���g�ɖ߂��Ă����Ƃ��ɖ炷�I�[�f�B�I�N���b�v"), SerializeField, ] AudioClip clipBeforeDie;
    [Header("Pause����O�̉���"), SerializeField] float volumeBeforePause;
    /// <summary>
    /// �I�[�f�B�I�}�l�[�W���[�̃C���X�^���X
    /// </summary>
    public static AudioManager Instance {  get; private set; }

    /// <summary>
    /// �|�[�Y���̃I�[�f�B�I�̉���
    /// </summary>
    public float VolumeWhilePause { get => volumeWhilePause; }

    private void Awake()
    {
        //�V���O���g��
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }

        //�v���C���[�����S���ăQ�[�������X�^�[�g�����Ƃ���bgm���`�F�b�N�|�C���g�ɂ����Ƃ��̂��̂ɕύX����B
        GameManager.Instance?.OnPlayerRestartObservable.Subscribe(_ => InitializeAudio()).AddTo(this);


    }

    /// <summary>
    /// AudioSource����n�߂����Ƃ����m����֐�
    /// </summary>
    /// <returns></returns>

    public bool IsAudioPlaying()
    {
        return audioSource.isPlaying;
    }

    /// <summary>
    /// bgm�����S�O�`�F�b�N�|�C���g��ʂ������ɗ���Ă���bgm�ɕύX����֐�
    /// </summary>

    void InitializeAudio()
    {
        //�I�[�f�B�I�\�[�X�̃N���b�v���`�F�b�N�|�C���g�ɐG�ꂽ�Ƃ��ɗ���Ă������̂ɐݒ肵�čĐ�����
        audioSource.clip = clipBeforeDie;
        audioSource.loop = true;
        audioSource.Play();
    }

    /// <summary>
    /// �Q�[�������X�^�[�g�������Ƃ��ɗ����bgm��o�^����֐�
    /// </summary>

    public void SetAudioBeforeDie()
    {
        //���ݐݒ肳��Ă�����̂����X�^�[�g���ɗ����N���b�v�Ƃ��ēo�^
        clipBeforeDie = audioSource.clip;
    }

    /// <summary>
    /// AudioSource��fadeout������֐�
    /// </summary>
    /// <param name="fadeOutTime"></param>

    public void FadeOut(float fadeOutTime)
    {
        audioSource.DOFade(0, fadeOutTime);
    }

    /// <summary>
    /// AudioSource���~�߂�֐�
    /// </summary>

    public void StopAudio()
    {
        audioSource.Pause();
    }

    /// <summary>
    /// AudioClip���w�肵�čĐ�����֐�
    /// </summary>
    /// <param name="clipName"></param>

    public void PlayAudio(AudioClipData.AudioClipName clipName)
    {

        audioSource.volume = 1f;
        //AudioClip�̃f�[�^����w�肵��clipname�Ɉ�v����bgmdata���擾����
        var data = audioClipData.BgmDatas.FirstOrDefault(data => data.Name == clipName);
        audioSource.clip = data.AudioClip; //�擾�����f�[�^��AudioClip��AudioSource�ɓo�^����

        //���S��������GameOver��������bgm�͉��x�������Ɛ�]���������Ă������ɂ��킢�����Ȃ̂�1�x���������悤�ɂ���
        if (clipName == AudioClipData.AudioClipName.DieBGM || clipName == AudioClipData.AudioClipName.GameOverBGM)
        {
            audioSource.loop = false;
        }

        //���̂ق���bgm�̓��[�v����悤�ɂ���
        else
        {
            audioSource.loop = true;
        }
        audioSource.Play();
    }

    /// <summary>
    /// SE���w�肵�Ė炷����
    /// </summary>
    /// <param name="clipName"></param>
    public void PlaySE(AudioClipData.SeAudioClipName clipName)
    {
        var se = audioClipData.SeDatas.Find(data => data.Name == clipName).AudioClip;
        seAjudioSource.PlayOneShot(se);
    }

    /// <summary>
    /// �|�[�Y�������ɉ��ʂ�������悤�ɂ���
    /// </summary>
    public void ChangeAudioToPauseMode()
    {
        volumeBeforePause = audioSource.volume;
        audioSource.volume = volumeWhilePause;
    }

    /// <summary>
    /// �|�[�Y�����������ɉ��ʂ�߂�
    /// </summary>

    public void ChangeAudioToUnPauseMode()
    {
        audioSource.volume = volumeBeforePause;
    }


}
