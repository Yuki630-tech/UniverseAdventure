using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UniRx;
using UniRx.Triggers;
using UnityEngine;


public class DialogueStarter : MonoBehaviour
{
    [Header("保存したファイルの名前をここにコピペしてください")]
    [Tooltip("開始させる会話文のファイル名"), SerializeField] private string fileName = ".csv";
    [Tooltip("コライダーのリスト"), SerializeField] private List<Collider> colliders = new List<Collider>();

    [Header("有効かどうか"), SerializeField] private bool isEnable;

    private void Awake()
    {
        isEnable = true;
        DialogueManager.Instance.OnDialogueDisposeObservable.Where(setFile => setFile == fileName).Subscribe(_ => isEnable = false, e => Debug.LogError($"会話キャンセルするファイルが見つかりませんでした。Assets/StreamingAssetsフォルダ内に指定のファイルを入れてください{e.Data}"));
        foreach (var collider in colliders)
        {
            collider.OnTriggerEnterAsObservable().Where(other => other.CompareTag("Player") && isEnable).Subscribe(async _ =>
            {
                isEnable = false;
                await DialogueManager.Instance.StartDialogue(fileName);
            });
        }
    }
}
