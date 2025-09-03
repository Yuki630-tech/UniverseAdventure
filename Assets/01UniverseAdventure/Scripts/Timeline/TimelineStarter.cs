using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineStarter : MonoBehaviour
{
    [Tooltip("作動させるTimeline"), SerializeField] private TimelineController timelineController;
    [Tooltip("コライダーのリスト"), SerializeField] private List<Collider> colliders;

    [Header("機能するかどうか"), SerializeField] private bool isEnable;

    private void Awake()
    {
        isEnable = true;
        GameManager.Instance.OnPlayerRestartObservable.Subscribe(_ => isEnable = true).AddTo(this);
        foreach (var collider in colliders)
        {
            collider.OnTriggerEnterAsObservable().Where(other => other.CompareTag("Player") && isEnable).Subscribe(_ =>
            {
                timelineController.StartTimeline();
                isEnable = false;
            });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isEnable)
        {
            timelineController.StartTimeline();
            isEnable = false;
        }
    }
}
