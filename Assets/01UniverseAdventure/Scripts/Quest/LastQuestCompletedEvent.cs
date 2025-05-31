using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class LastQuestCompletedEvent : MonoBehaviour
{
    [SerializeField] CollectQuest collectQuest;
    [SerializeField] PauseUI pauseUI;
    
    private void Awake()
    {
        collectQuest.OnCompleteQuestObservable.Subscribe(_ => OnGameEnd()).AddTo(this);
    }

    void OnGameEnd()
    {
        AudioManager.Instance?.PlayAudio(AudioClipData.AudioClipName.TitleBGM);
        pauseUI.ChangeIfEnableToPause(false);

    }
}
