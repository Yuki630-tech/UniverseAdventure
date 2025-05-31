using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class GravityChanger : MonoBehaviour
{
    ReactiveProperty<float> gravityDirection = new ReactiveProperty<float>();
    [SerializeField] AudioChangeGravitySEPlayer audioChangeGravitySEPlayer;
    [SerializeField] CollectQuest collectQuest;
    [Tooltip("�x����ʒm����Ԋu"), SerializeField] float warningInterval = 3.5f;
    [Tooltip("�|�[�Y���������Ă���x����ʒm����܂ł̊Ԋu"), SerializeField] float warningIntervalFromUnPause = 1.3f;
    [Tooltip("�x������炷�Ԋu"), SerializeField] float warningSEPlayInterval = 0.5f;
    int warningCount = 3; //�P�x�̌x���ŉ���炷��
    IDisposable playerDieDisposable;
    IDisposable gameClearDisposable;
    [SerializeField] PauseUI pauseUI;
    [Header("Pause������ĊJ�����ۂɏd�͐؂�ւ����ĊJ�����邩�ǂ���"), SerializeField] bool isResumableAfterPause;
    [Header("�R���[�`���ĊJ�p�̃t���O"), SerializeField] bool isNotPausing;
    /// <summary>
    /// �d�͐؂�ւ��̃R���[�`���B��~�ł���悤�ϐ��ɕێ����Ă���
    /// </summary>
    public Coroutine GravityChangeCoroutine { get; private set; }

    /// <summary>
    /// ��莞�Ԃ��Ƃ�+1/-1�Ɛ؂�ւ��d�͂̌����̃v���p�e�B�B+1���ƃv���C���[�̏�����������l�̕�����,-1�ł͋t�̕����Ɍ����B
    /// </summary>
    public IReadOnlyReactiveProperty<float> GravityDirection => gravityDirection;

    private void Awake()
    {
        isNotPausing = true;
        isResumableAfterPause = true;
        //�N�G�X�g�m���}��B�������Ƃ��ɗ���鉉�o�̊J�n���ɏd�͐؂�ւ����~�߁A�v���C���[���Ăѓ�����悤�ɂȂ�����ĊJ����悤�ɂ���悤��
        collectQuest.OnStopQuestObservable.Subscribe(_ => StopGravityChange()).AddTo(gameObject);
        collectQuest.OnQuitQuestObservable.Subscribe(_ =>
        {
            StopGravityChange();
            isResumableAfterPause = false;
        }).AddTo(gameObject);
        collectQuest.OnReplayQuestObservable.Subscribe(_ => RestartGravityChange()).AddTo(gameObject);


    }

    /// <summary>
    /// ��莞�Ԃ��Ƃ̏d�͐؂�ւ����J�n����֐�(SetGravityChanger�I�u�W�F�N�g�Ƀv���C���[���G�ꂽ�Ƃ���UnityEvent�Ƃ��ēo�^)
    /// </summary>
    public void StartGravityChange()
    {
        //�ŏ��͏�����Ƀv���C���[�������悤��
        gravityDirection.Value = 1;
        GravityChangeCoroutine = StartCoroutine(GravityChange());
        //�N���A�A�v���C���[���S���ɏd�͐؂�ւ����~����悤�ɂ���
        playerDieDisposable = GameManager.Instance?.OnPlayerDieObservable.Subscribe(_ => StopGravityChange()).AddTo(this);
        gameClearDisposable = GameManager.Instance?.OnGameClearOrOverObservable.Subscribe(_ => StopGravityChange()).AddTo(this);
        GameManager.Instance?.OnPauseGameObservable.Subscribe(_ => isNotPausing = false).AddTo(this);
        GameManager.Instance?.OnUnPauseGameObservable.Where(_ => isResumableAfterPause).Subscribe(_ => isNotPausing = true).AddTo(this);
    }

    /// <summary>
    /// �N�G�X�g�m���}�B����̉��o���I���������Ɉ�莞�Ԃ��Ƃ̏d�͐؂�ւ����ĊJ����֐�
    /// </summary>
    public void RestartGravityChange()
    {
        Debug.Log("�d�͐؂�ւ��ĊJ");
        GravityChangeCoroutine = StartCoroutine(GravityChange());
        playerDieDisposable = GameManager.Instance?.OnPlayerDieObservable.Subscribe(_ => StopGravityChange()).AddTo(this);
        gameClearDisposable = GameManager.Instance?.OnGameClearOrOverObservable.Subscribe(_ => StopGravityChange()).AddTo(this);
    }

    /// <summary>
    /// ��莞�Ԃ��Ƃ̏d�͐؂�ւ����~����֐�
    /// </summary>
    public void StopGravityChange()
    {
        if (GravityChangeCoroutine != null)
        {
            StopCoroutine(GravityChangeCoroutine);
            GravityChangeCoroutine = null;
            playerDieDisposable.Dispose();
            gameClearDisposable.Dispose();
        }


    }
    /// <summary>
    /// ��莞�Ԃ��Ƃɏd�͂�؂�ւ���R���[�`��
    /// </summary>
    /// <returns></returns>
    IEnumerator GravityChange()
    {
    
        while (true)
        {
            yield return new WaitUntil(() => isNotPausing);
            yield return new WaitForSeconds(warningInterval);
            if (!isNotPausing)
            {
                yield return new WaitUntil(() => isNotPausing);
                yield return new WaitForSeconds(warningIntervalFromUnPause);
            }
            for (int i = 0; i < warningCount; i++)
            {
                audioChangeGravitySEPlayer.PlayWarningGravitySE();
                yield return new WaitForSeconds(warningSEPlayInterval);
            }
            //���A�N�e�B�u�v���p�e�B�̒l��-
            gravityDirection.Value = -gravityDirection.Value;
            audioChangeGravitySEPlayer.PlayChangeGravitySE();
        }
    }
}
