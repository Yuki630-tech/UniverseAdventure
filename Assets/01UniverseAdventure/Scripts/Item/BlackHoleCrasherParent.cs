using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

public class BlackHoleCrasherParent : MonoBehaviour
{
    [Tooltip("BlackHoleCrasherを映すVirtualCamera"), SerializeField] private CinemachineVirtualCamera blackHoleCrasherCamera;
    [Tooltip("プレイヤーのカメラに戻すまでの時間"), SerializeField] private float backToPlayerCameraInterval = 2f;
    [Tooltip("戻すさきのプレイヤーカメラ"), SerializeField] private string playerCameraName;
    async void OnEnable()
    {
        CameraManager.Instance.ChangeCamera(blackHoleCrasherCamera, CinemachineBlendDefinition.Style.Cut, 0f);
        GameManager.Instance.OnPauseGame();
        await UniTask.Delay(TimeSpan.FromSeconds(backToPlayerCameraInterval));
        GameManager.Instance.OnUnPauseGame();
        CameraManager.Instance.ChangeCamera(playerCameraName, CinemachineBlendDefinition.Style.Cut, 0f);
    }
}
