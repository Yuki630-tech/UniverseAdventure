using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SmallPlanetArea : MonoBehaviour
{
    [Tooltip("�v���C���[�̃g�����X�t�H�[��"), SerializeField] private Transform playerTrans;
    [Tooltip("���̃X�e�[�W�ɂ�����N�G�X�g(�N���A���̒ʒm���󂯎�邽�߂ɕێ����Ă���)"), SerializeField] private CollectQuest collectQuest;
    [Tooltip("���s�Ƃ݂Ȃ�����"), SerializeField] private float parallelDot = 0.98f;

    public enum PlayerDirection
    {
        Up,
        Down
    }

    private ReactiveProperty<float> dotBetweenPlayerAndUpProperty = new ReactiveProperty<float>();

    private void Awake()
    {
        dotBetweenPlayerAndUpProperty.Where(dotBetweenPlayerAndUp => dotBetweenPlayerAndUp >= parallelDot).Subscribe()
    }

    private void Update()
    {
        dotBetweenPlayerAndUpProperty.Value = Vector3.Dot(playerTrans.up, Vector3.up);
    }
}
