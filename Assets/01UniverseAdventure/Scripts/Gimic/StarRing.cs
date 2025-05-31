using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UniRx;

public class StarRing : MonoBehaviour
{
    [Tooltip("再生するスプライン"), SerializeField] SplineContainer spline;
    bool isEffective;

    [Tooltip("クエスト。演出時スターリングに勝手に乗らないようにするため取得"), SerializeField] CollectQuest collectQuest;

    private void Awake()
    {
        isEffective = true;
        collectQuest?.OnStopQuestObservable.Subscribe(_ => isEffective = false);
        collectQuest?.OnReplayQuestObservable.Subscribe(_ => isEffective = true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isEffective)
        {
            var player = other.gameObject.GetComponent<Player>();
            if (player == null) return;

            player.ChangeToFlyState(spline);
        }
        

    }
}
