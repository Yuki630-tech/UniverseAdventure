using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UniRx;
using Cinemachine;

public class TimelineController : MonoBehaviour
{
    [Tooltip("ì“®‚³‚¹‚éPlayableDirector"), SerializeField] private PlayableDirector playableDirector;
    [Header("‰ï˜b‚ğ’†’f‚³‚¹‚½‚Æ‚«‚É—Dæ“x‚ğ-1‚É‚·‚é‚½‚ßTimeline‚Åg‚¤‚·‚×‚Ä‚ÌVirtualCamera‚ğ‚±‚±‚ÉŠi”[‚µ‚Ä‚­‚¾‚³‚¢"), SerializeField]
    private List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();
   
    public void StartTimeline()
    {
        playableDirector.Play();
        DebugLog.Log($"CameraManager.Instance‚Í‚¿‚á‚ñ‚Æ‚ ‚è‚Ü‚·‚©? : {CameraManager.Instance != null}");
        DialogueManager.Instance.OnDialogueEndObservable.Subscribe(_ => ResetCamera());
    }

    public void PauseTimeline()
    {
        playableDirector.Pause();
    }

    public void ResetCamera()
    {
        playableDirector.Stop();
        foreach(var cam in cameras)
        {
            cam.Priority = -1;
        }
    }
}
