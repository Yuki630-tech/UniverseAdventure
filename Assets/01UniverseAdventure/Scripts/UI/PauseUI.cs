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
    [Tooltip("ゲームのポーズ時に表示させるUI"), SerializeField] GameObject pauseUI;
    [Tooltip("ポーズする操作を受け取るためにinputManagerを取得しておく"), SerializeField] InputManager inputManager;
    [Tooltip("終了ボタン"), SerializeField] Button quitButton;
    [Tooltip("操作説明ボタン"), SerializeField] Button descriptionButton;

    [Tooltip("操作説明UI"), SerializeField] GameObject descriptionUI;
    bool isDescriptionOpen;
    [SerializeField] bool canOpenPauseMenu; //死亡の演出中はポーズできないようにするためのフラグ
    // Start is called before the first frame update
    void Start()
    {
        //最初はポーズ画面を開けるように
        canOpenPauseMenu = true;
        pauseUI.SetActive(false);
        GameManager.Instance?.OnPlayerDieObservable.Subscribe(_ => canOpenPauseMenu = false).AddTo(gameObject); //死亡演出中はポーズ画面を開けないようにする
        GameManager.Instance?.OnPlayerRestartObservable.Subscribe(_ => canOpenPauseMenu = true).AddTo(gameObject); //リスタートした時に開けるようにする
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
    /// ゲームポーズ
    /// </summary>
    void PauseGame()
    {
        GameManager.Instance?.OnPauseGame();
        AudioManager.Instance?.ChangeAudioToPauseMode();
        pauseUI.SetActive(true);
        DebugLog.Log("ポーズ");
    }

    /// <summary>
    /// ゲームアンポーズ
    /// </summary>
    void UnPauseGame()
    {
        GameManager.Instance?.OnUnPauseGame();
        AudioManager.Instance?.ChangeAudioToUnPauseMode();
        pauseUI.SetActive(false);
        DebugLog.Log("アンポーズ");
    }

    /// <summary>
    /// 操作説明表示
    /// </summary>
    void OpenDescription()
    {
        isDescriptionOpen = true;
        descriptionUI.SetActive(true);
        quitButton.interactable = false;
        descriptionButton.interactable = false;
    }
    /// <summary>
    /// 操作説明非表示
    /// </summary>
    void CloseDescription()
    {
        isDescriptionOpen = false;
        descriptionUI.SetActive(false);
        quitButton.interactable = true;
        descriptionButton.interactable = true;
        descriptionButton.Select(); //非表示にした時に操作説明のボタンが選択された状態にする。
    }

    public void ChangeIfEnableToPause(bool setEnable)
    {
        canOpenPauseMenu = setEnable;
    }
}
