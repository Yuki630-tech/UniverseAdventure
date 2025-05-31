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
    [Tooltip("スタートボタン"), SerializeField] Button startButton;
    [Tooltip("終了ボタン"), SerializeField] Button quitButton;
    [Tooltip("操作説明UIを表示させるボタン"), SerializeField] Button descriptionButton;
    [Tooltip("操作説明UI"), SerializeField] GameObject descriptionUI;
    [Tooltip("キー入力をしたときに操作説明画面を閉じられるようにinputManagerを取得"), SerializeField] InputManager inputManager;
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
