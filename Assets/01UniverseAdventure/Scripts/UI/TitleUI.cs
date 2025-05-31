using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using UniRx;
using Cysharp.Threading.Tasks;

public class TitleUI : MonoBehaviour
{
    [Tooltip("�X�^�[�g�{�^��"), SerializeField] Button startButton;
    [Tooltip("�I���{�^��"), SerializeField] Button quitButton;
    [Tooltip("�������UI��\��������{�^��"), SerializeField] Button descriptionButton;
    [Tooltip("�������UI"), SerializeField] GameObject descriptionUI;
    [Tooltip("�L�[���͂������Ƃ��ɑ��������ʂ������悤��inputManager���擾"), SerializeField] InputManager inputManager;
    bool isOpen;

    private void Awake()
    {
        descriptionUI.SetActive(false);
        startButton.OnClickAsObservable().Subscribe(_ => GameManager.Instance?.StartGame()).AddTo(this);
        quitButton.OnClickAsObservable().Subscribe(_ =>GameManager.Instance?.QuitGame()).AddTo(this);
        descriptionButton.OnClickAsObservable().Subscribe(_ => OpenDescription()).AddTo(this);
        inputManager.IsMenuOpenClose.Where(isClose => isClose && isOpen).Subscribe(_ => CloseDescription()).AddTo(this);
    }

    void OpenDescription()
    {
        isOpen = true;
        descriptionUI.SetActive(true);
        startButton.interactable = false;
        quitButton.interactable = false;
        descriptionButton.interactable = false;
    }

    void CloseDescription()
    {
        isOpen = false;
        descriptionUI.SetActive(false);
        startButton.interactable = true;
        quitButton.interactable = true;
        descriptionButton.interactable= true;
        descriptionButton.Select();
    }
   
}
