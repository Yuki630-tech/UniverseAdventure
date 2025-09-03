using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Cysharp.Threading.Tasks;
using Unity.Properties;
using System.Threading;
using UniRx;

public class DialogueManager : MonoBehaviour
{
    [Header("DialogueUIの設定")]
    [Tooltip("DialogueUI"), SerializeField] private DialogueUI dialogueUI;
    [Tooltip("InputManager"), SerializeField] private InputManager inputManager;
    List<IEventListener> listeners = new List<IEventListener>();
    private bool isEnableToCancel;
    private bool isTalking;

    private Subject<Unit> onDialogueStartSubject = new Subject<Unit>();
    private Subject<Unit> onDialogueEndSubject = new Subject<Unit>();
    private Subject<string> onDialogueDisposeSubject = new Subject<string>();
    public IObservable<Unit> OnDialogueStartObservable => onDialogueStartSubject;
    public IObservable<Unit> OnDialogueEndObservable => onDialogueEndSubject;
    public IObservable<string> OnDialogueDisposeObservable => onDialogueDisposeSubject;

    IDisposable disposable;
    CancellationTokenSource cts = new CancellationTokenSource();

    public static DialogueManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        cts = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        cts.Cancel();
        cts.Dispose();
    }

    public void PrepareForDialogue(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (!File.Exists(path))
        {
            Debug.LogError($"ファイルが見つかりません。指定ファイルをAssets/StreamingAssetsフォルダーに入れてください{path}");
            return;
        }
        string[] strings = File.ReadAllLines(path);
        string[] line1 = strings[0].Split(",");
        int guidIndex = Array.IndexOf(line1, "ガイド");
        string[] line2 = strings[1].Split(",");
        string guidString = line2[guidIndex];
        dialogueUI.ShowGuid(guidString);
    }

    public void HideGuid()
    {
        dialogueUI.HideGuid();
    }
    public async UniTask StartDialogue(string fileName)
    {
        try
        {
            disposable = inputManager.IsDecided.Where(isDecided => isDecided).Subscribe(_ => SkipDialogue(fileName)).AddTo(gameObject);
            string path = Path.Combine(Application.streamingAssetsPath, fileName);
            string[] strings = File.ReadAllLines(path);
            string[] line1 = strings[0].Split(',');
            int idIndex = Array.IndexOf(line1, "ID");
            int characterNameIndex = Array.IndexOf(line1, "キャラクター名");
            int serifIndex = Array.IndexOf(line1, "セリフ");
            int isEndIndex = Array.IndexOf(line1, "会話終了か");
            int isBranchIndex = Array.IndexOf(line1, "選択肢があるか");
            int branch1Index = Array.IndexOf(line1, "選択肢1");
            int branch1SkipIdIndex = Array.IndexOf(line1, "選択肢1を選んだ場合のスキップID");
            int branch2Index = Array.IndexOf(line1, "選択肢2");
            int branch2SkipIdIndex = Array.IndexOf(line1, "選択肢2を選んだ場合のスキップID");
            int eventIdIndex = Array.IndexOf(line1, "イベントID");
            int typingIntervalIndex = Array.IndexOf(line1, "タイピング間隔");
            int goToNextSentenceIntervalIndex = Array.IndexOf(line1, "次の文に行くまでの間隔");
            onDialogueStartSubject.OnNext(Unit.Default);
            for (int i = 1; i < strings.Length - 1; i++)
            {
                string[] contents = strings[i].Split(',');
                if (contents[eventIdIndex] != "")
                {
                    var dialogueEventListener = listeners.Find(listener => listener.GetId() == contents[eventIdIndex]);
                    if (dialogueEventListener != null)
                    {
                        dialogueEventListener.Invoke();
                    }

                }

                await dialogueUI.ShowSentenceTask(contents[characterNameIndex], contents[serifIndex], float.Parse(contents[typingIntervalIndex]), cts.Token);
                await UniTask.WaitUntil(() => inputManager.IsGoToNextSerif.Value, cancellationToken: cts.Token);
            }

            var lastDialogueEventListener = listeners.Find(listener => listener.GetId() == strings[strings.Length - 1].Split(',')[eventIdIndex]);
            if (strings[strings.Length - 1].Split(",")[eventIdIndex] != "" && lastDialogueEventListener != null)
            {
                lastDialogueEventListener.Invoke();

            }

            else
            {
                string[] contents = strings[strings.Length - 1].Split(',');
                await dialogueUI.ShowSentenceTask(contents[characterNameIndex], contents[serifIndex], float.Parse(contents[typingIntervalIndex]), cts.Token);
                await UniTask.WaitUntil(() => inputManager.IsGoToNextSerif.Value, cancellationToken: cts.Token);
            }
            isTalking = false;
            disposable.Dispose();
            onDialogueEndSubject.OnNext(Unit.Default);

            dialogueUI.HideSentence();
        }

        catch (FileNotFoundException e)
        {
            Debug.LogError($"ファイルが存在しません。Assets/StreamingAssets内に指定したファイルがあることを確認してください {e.FileName}");
        }

        catch (OperationCanceledException e)
        {
            Debug.Log($"StartDialogue()が途中で終了しました{e.GetType()}");
        }
    }

    [ContextMenu("デバッグログに出力してみよう")]
    private async UniTaskVoid TestDialogue()
    {
        await StartDialogue("Scenario11.csv");
    }

    public void AddEventListener(IEventListener eventListener)
    {
        listeners.Add(eventListener);
    }

    public void RemoveEventListener(IEventListener eventListener)
    {
        if (listeners.Contains(eventListener))
        {
            listeners.Remove(eventListener);
        }
    }

    public void DisposeDialogue(string fileName)
    {
        try
        {
            onDialogueDisposeSubject.OnNext(fileName);
        }

        catch (FileNotFoundException e)
        {
            onDialogueDisposeSubject.OnError(e);
        }
    }

    private void SkipDialogue(string fileName)
    {
        cts.Cancel();
        cts.Dispose();
        cts = new CancellationTokenSource();

        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        string[] strings = File.ReadAllLines(path);
        string[] line1 = strings[0].Split(',');
        int eventIdIndex = Array.IndexOf(line1, "イベントID");
        var lastDialogueEventListener = listeners.Find(listener => listener.GetId() == strings[strings.Length - 1].Split(',')[eventIdIndex]);
        dialogueUI.HideSentence();
        onDialogueEndSubject.OnNext(Unit.Default);
        if (lastDialogueEventListener == null) return;
        lastDialogueEventListener.Invoke();
        disposable.Dispose();
    }
}
