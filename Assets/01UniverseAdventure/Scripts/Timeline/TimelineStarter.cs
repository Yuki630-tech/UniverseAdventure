using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineStarter : MonoBehaviour
{
    [Tooltip("作動させるTimeline"), SerializeField] private PlayableDirector playableDirector;

    [Header("機能するかどうか"), SerializeField] private bool isEnable;

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
