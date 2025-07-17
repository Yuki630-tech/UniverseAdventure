using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineStarter : MonoBehaviour
{
    [Tooltip("ì“®‚³‚¹‚éTimeline"), SerializeField] private PlayableDirector playableDirector;

    [Header("‹@”\‚·‚é‚©‚Ç‚¤‚©"), SerializeField] private bool isEnable;

    private void Awake()
    {
        isEnable = true;
        GameManager.Instance.OnPlayerRestartObservable.Subscribe(_ => isEnable = true).AddTo(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isEnable)
        {
            playableDirector.Play();
            isEnable = false;
        }
    }
}
