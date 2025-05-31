using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �A�N�e�B�u�ɂȂ������Ɏ��g���J�����}�l�[�W���[�ɓo�^�A��A�N�e�B�u�ɂȂ������Ɏ��g���J�����}�l�[�W���[����폜����N���X
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
