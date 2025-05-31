using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アクティブになった時に自身をカメラマネージャーに登録、非アクティブになった時に自身をカメラマネージャーから削除するクラス
/// </summary>
public class VirtualCamera : MonoBehaviour
{
    private void OnEnable()
    {
        
        CameraManager.Instance?.cinemachineVirtualCameras.Add(GetComponent<CinemachineVirtualCamera>());
    }

    private void OnDisable()
    {
        CameraManager.Instance?.cinemachineVirtualCameras.Remove(GetComponent<CinemachineVirtualCamera>());
    }

}
