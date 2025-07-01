using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SmallPlanetArea : MonoBehaviour
{
    [Tooltip("プレイヤーのトランスフォーム"), SerializeField] private Transform playerTrans;
    [Tooltip("このステージにおけるクエスト(クリア時の通知を受け取るために保持しておく)"), SerializeField] private CollectQuest collectQuest;
    [Tooltip("平行とみなす内積"), SerializeField] private float parallelDot = 0.98f;

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
