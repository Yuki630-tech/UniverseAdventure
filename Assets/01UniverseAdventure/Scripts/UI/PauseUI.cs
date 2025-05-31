using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime;
using Cysharp.Threading.Tasks;

public class PauseUI : MonoBehaviour
{
    [Tooltip("�Q�[���̃|�[�Y���ɕ\��������UI"), SerializeField] GameObject pauseUI;
    [Tooltip("�|�[�Y���鑀����󂯎�邽�߂�inputManager���擾���Ă���"), SerializeField] InputManager inputManager;
    [Tooltip("�I���{�^��"), SerializeField] Button quitButton;
    [Tooltip("��������{�^��"), SerializeField] Button descriptionButton;

    [Tooltip("�������UI"), SerializeField] GameObject descriptionUI;
    bool isDescriptionOpen;
    [SerializeField] bool canOpenPauseMenu; //���S�̉��o���̓|�[�Y�ł��Ȃ��悤�ɂ��邽�߂̃t���O
    // Start is called before the first frame update
    void Start()
    {
        //�ŏ��̓|�[�Y��ʂ��J����悤��
        canOpenPauseMenu = true;
        pauseUI.SetActive(false);
        GameManager.Instance?.OnPlayerDieObservable.Subscribe(_ => canOpenPauseMenu = false).AddTo(gameObject); //���S���o���̓|�[�Y��ʂ��J���Ȃ��悤�ɂ���
        GameManager.Instance?.OnPlayerRestartObservable.Subscribe(_ => canOpenPauseMenu = true).AddTo(gameObject); //���X�^�[�g�������ɊJ����悤�ɂ���
        isDescriptionOpen = false;
        descriptionUI.SetActive(false);
        quitButton.OnClickAsObservable().Subscribe(_ => GameManager.Instance?.QuitGame()).AddTo(gameObject);
        descriptionButton.OnClickAsObservable().Subscribe(_ => OpenDescription()).AddTo(gameObject);
    }

    private void Update()
    {
        if (inputManager.IsMenuOpenClose.Value && canOpenPauseMenu)
        {
            if (!isDescriptionOpen && (GameManager.Instance != null && GameManager.Instance.IsPausing))
            {
                UnPauseGame();
            }

            else if (isDescriptionOpen)
            {
                CloseDescription();
            }

            else
            {
                PauseGame();
            }
        }
    }

    /// <summary>
    /// �Q�[���|�[�Y
    /// </summary>
    void PauseGame()
    {
        GameManager.Instance?.OnPauseGame();
        AudioManager.Instance?.ChangeAudioToPauseMode();
        pauseUI.SetActive(true);
        DebugLog.Log("�|�[�Y");
    }

    /// <summary>
    /// �Q�[���A���|�[�Y
    /// </summary>
    void UnPauseGame()
    {
        GameManager.Instance?.OnUnPauseGame();
        AudioManager.Instance?.ChangeAudioToUnPauseMode();
        pauseUI.SetActive(false);
        DebugLog.Log("�A���|�[�Y");
    }

    /// <summary>
    /// ��������\��
    /// </summary>
    void OpenDescription()
    {
        isDescriptionOpen = true;
        descriptionUI.SetActive(true);
        quitButton.interactable = false;
        descriptionButton.interactable = false;
    }
    /// <summary>
    /// ���������\��
    /// </summary>
    void CloseDescription()
    {
        isDescriptionOpen = false;
        descriptionUI.SetActive(false);
        quitButton.interactable = true;
        descriptionButton.interactable = true;
        descriptionButton.Select(); //��\���ɂ������ɑ�������̃{�^�����I�����ꂽ��Ԃɂ���B
    }

    public void ChangeIfEnableToPause(bool setEnable)
    {
        canOpenPauseMenu = setEnable;
    }
}
