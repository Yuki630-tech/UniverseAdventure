using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBase : MonoBehaviour, IDialogueProvider
{
    [Tooltip("会話文のファイル名"), SerializeField] private string dialogueFileName = ".csv";

    public string GetFileName()
    {
        return dialogueFileName;
    }

    public void SetFileName(string setName)
    {
        dialogueFileName = setName;
    }
}
