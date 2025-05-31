using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    [Tooltip("切り替わる前のカメラ、戻って来て再度触れた際に設定するカメラの名前"),SerializeField] string cameraName1;
    [Tooltip("camera1→camera2に切り替わる際のblendモード"),SerializeField] CinemachineBlendDefinition.Style customBlend12;
    [Tooltip("camera1→camera2。ブレンドにかける時間"), SerializeField] float blendTime12;
    [Tooltip("切り替え先のカメラの名前"), SerializeField] string cameraName2;
    [Tooltip("camera2→camera1。ブレンドモード"),SerializeField] CinemachineBlendDefinition.Style customBlend21;
    [Tooltip("camera2→camera1。 ブレンドにかける時間"),SerializeField] float blendTime21;
    [Tooltip("プレイヤーが戻ってきて再度通過した際にカメラを戻す機能があるかどうか"),SerializeField] bool hasRecoverEffect;
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            //現在のカメラの名前がcameraName1の場合cameraName2のカメラに切り替える
            if (CameraManager.Instance?.GetCurrentCamera() == CameraManager.Instance?.GetCamera(cameraName1))
            {
                CameraManager.Instance?.ChangeCamera(cameraName2, customBlend12, blendTime12);
                
            }

            //現在のカメラの名前がcameraName2でカメラを戻す機能がある場合にはcameraName1にカメラを戻す。
            else if(CameraManager.Instance?.GetCurrentCamera() == CameraManager.Instance?.GetCamera(cameraName2) && hasRecoverEffect)
            {
                CameraManager.Instance?.ChangeCamera(cameraName1, customBlend21, blendTime21);
            }
        }
    }
}
