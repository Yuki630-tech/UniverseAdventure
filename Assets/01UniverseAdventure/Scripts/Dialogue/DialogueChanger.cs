using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueChanger : MonoBehaviour
{
    [Tooltip("変更する前の会話文のファイル名"), SerializeField] private string beforeFileName = ".csv";
    [Tooltip("変更する先のファイル名"), SerializeField] private string fileName = ".csv";
    [Tooltip("変更するDialogueProvider"), SerializeField] private NPCBase dialogueProvider;

    public void ChangeDialogueFile()
    {
        dialogueProvider.SetFileName(fileName);
        DialogueManager.Instance.DisposeDialogue(beforeFileName);
    }
}
