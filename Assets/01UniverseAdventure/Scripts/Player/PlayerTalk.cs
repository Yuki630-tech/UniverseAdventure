using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTalk : MonoBehaviour
{
    [Tooltip("DialogueProviderChecker"), SerializeField] private DialogueProviderChecker dialogueProviderChecker;
    [Tooltip("InputManager"), SerializeField] private InputManager inputManager;
    private bool isEnable;
    // Start is called before the first frame update
    void Start()
    {
        isEnable = true;
    }

    // Update is called once per frame
    async void Update()
    {
        var dialogueProvider = dialogueProviderChecker.GetDialogueProvider();

        if(dialogueProvider != null)
        {
            DialogueManager.Instance.PrepareForDialogue(dialogueProvider.GetFileName());
            if (inputManager.IsGoToNextSerif.Value && isEnable)
            {
                isEnable = false;
                await DialogueManager.Instance.StartDialogue(dialogueProvider.GetFileName());
            }
        }

        else
        {
            DialogueManager.Instance.HideGuid();
            isEnable = true;
        }
    }
}
