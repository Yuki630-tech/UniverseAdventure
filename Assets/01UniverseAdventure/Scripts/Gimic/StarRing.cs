using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using UniRx;

public class StarRing : MonoBehaviour
{
    [Tooltip("�Đ�����X�v���C��"), SerializeField] SplineContainer spline;
    bool isEffective;

    [Tooltip("�N�G�X�g�B���o���X�^�[�����O�ɏ���ɏ��Ȃ��悤�ɂ��邽�ߎ擾"), SerializeField] CollectQuest collectQuest;

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
