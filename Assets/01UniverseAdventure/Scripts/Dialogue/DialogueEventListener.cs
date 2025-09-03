using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueEventListener : MonoBehaviour, IEventListener
{
    [Tooltip("イベントID"), SerializeField] private string eventId;

    [Header("このIDで発火させたいイベント"), SerializeField]
    private UnityEvent unityEvent;

    public string GetId()
    {
        return eventId;
    }

    public void Invoke()
    {
        unityEvent.Invoke();
    }

    private void OnEnable()
    {
        DialogueManager.Instance.AddEventListener(this);
    }

    private void OnDisable()
    {
        DialogueManager.Instance.RemoveEventListener(this);
    }
}
