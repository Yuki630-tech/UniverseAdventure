using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;


public class DialogueUI : MonoBehaviour
{
    [Header("会話文のパネル")]
    [Tooltip("会話文のセット"), SerializeField] private GameObject dialogueUI;
    [Tooltip("キャラクター名を表示させるテキスト"), SerializeField] private TextMeshProUGUI characterNameText;
    [Tooltip("セリフを表示させるテキスト"), SerializeField] private TextMeshProUGUI dialogueTest;
    

    [Header("選択肢のパネル")]
    [Tooltip("選択肢のセット"), SerializeField] private GameObject branchUI;
    [Tooltip("選択肢1のテキスト"), SerializeField] private TextMeshProUGUI branch1Text;
    [Tooltip("選択肢2のテキスト"), SerializeField] private TextMeshProUGUI branch2Text;

    [Header("入力の設定")]
    [Tooltip("InputManager"), SerializeField] private InputManager inputManager;

    [Header("会話案内のパネル")]
    [Tooltip("会話できることを知らせるUI"), SerializeField] private GameObject dialogueGuidUI;
    [Tooltip("会話できることを知らせるテキスト"), SerializeField] private TextMeshProUGUI dialogueGuidText;

    CancellationTokenSource cts = new CancellationTokenSource();

    private void Awake()
    {
        dialogueUI.SetActive(false);
        dialogueGuidUI.SetActive(false);
    }
    public async UniTask ShowSentenceTask(string setCharacterName, string setText, float typeInterval, CancellationToken ct)
    {
        CancellationTokenSource delayCts = new CancellationTokenSource();
        using(var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, delayCts.Token))
        {
            try
            {
                dialogueUI.SetActive(true);
                characterNameText.text = setCharacterName;
                dialogueTest.text = "";
                for (int i = 0; i < setText.Length; i++)
                {
                    dialogueTest.text += setText[i];
                    UniTask typeTask = UniTask.Delay(TimeSpan.FromSeconds(typeInterval), cancellationToken: linkedCts.Token);
                    UniTask goToNextSerifInputTask = UniTask.WaitUntil(() => inputManager.IsGoToNextSerif.Value);
                    await UniTask.WhenAny(typeTask, goToNextSerifInputTask);
                    if (inputManager.IsGoToNextSerif.Value)
                    {
                        ShowAllSentence(setText);
                        break;
                    }
                }
            }

            catch (OperationCanceledException e)
            {
                Debug.Log("ShowSentenceTask()が中断しました");
            }
        }
    }

    public bool IsAllSentenceShowed(string setText)
    {
        return dialogueTest.text == setText;
    }
    public void ShowAllSentence(string setText)
    {
        dialogueTest.text = setText;
    }

    public void HideSentence()
    {
        dialogueUI.SetActive(false);
    }

    public void ShowChoices(string setBranch1, string setBranch2)
    {
        branch1Text.text = setBranch1;
        branch2Text.text = setBranch2;
    }

    public void ShowGuid(string setGuid)
    {
        dialogueGuidUI.SetActive(true);
        dialogueGuidText.text = setGuid;
    }

    public void HideGuid()
    {
        dialogueGuidUI.SetActive(false);
    }
}
