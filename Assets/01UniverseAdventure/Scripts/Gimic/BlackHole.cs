using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System;

public class BlackHole : MonoBehaviour
{
    bool isEffective; //�v���C���[���G�ꂽ���Ƃ����m���邩�ǂ���
    [SerializeField] AudioSource audioSource;

    private void Awake()
    {
        isEffective = true;
        //���X�^�[�g�������ɋ@�\��L���ɂ���
        GameManager.Instance?.OnPlayerRestartObservable.Where(_ => !isEffective).Subscribe(_ => isEffective = true).AddTo(this);
        //�v���C���[�����񂾂Ƃ��A�V���ȃu���b�N�z�[���ɍēx�z�����܂�Ă܂����S�������s�����Ƃ��Ȃ��悤�ɋ@�\���~���Ă���
        GameManager.Instance?.OnPlayerDieObservable.Subscribe(_ => isEffective = false).AddTo(this);
        GameManager.Instance?.OnGameClearOrOverObservable.Subscribe(_ => isEffective = false).AddTo(this);

        //�|�[�Y�������ɉ��ʂ𗎂Ƃ��A�|�[�Y�����������ɉ��ʂ�߂��B
        GameManager.Instance?.OnPauseGameObservable.Where(_ => AudioManager.Instance != null).Subscribe(_ => audioSource.volume = AudioManager.Instance.VolumeWhilePause).AddTo(this);
        GameManager.Instance?.OnUnPauseGameObservable.Subscribe(_ => audioSource.volume = 1f).AddTo(this);
    }

    /// <summary>
    /// �v���C���[���u���b�N�z�[���ɋz�����܂��͈͓��ɓ��������ɍs������
    /// </summary>
    /// <param name="other"></param>
    public void OnPlayerTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();

        if(player != null && isEffective)
        {
            //�u���b�N�z�[���ɋz�����܂�鋓�����J�n����
            player.ChangeToBlackHoleState(transform);
            
        }
    }

    /// <summary>
    /// �v���C���[���f�b�h�]�[���ɓ��������ɍs������
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
