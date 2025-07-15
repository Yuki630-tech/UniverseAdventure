using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    [Tooltip("�؂�ւ��O�̃J�����A�߂��ė��čēx�G�ꂽ�ۂɐݒ肷��J�����̖��O"),SerializeField] string cameraName1;
    [Tooltip("camera1��camera2�ɐ؂�ւ��ۂ�blend���[�h"),SerializeField] CinemachineBlendDefinition.Style customBlend12;
    [Tooltip("camera1��camera2�B�u�����h�ɂ����鎞��"), SerializeField] float blendTime12;
    [Tooltip("�؂�ւ���̃J�����̖��O"), SerializeField] string cameraName2;
    [Tooltip("camera2��camera1�B�u�����h���[�h"),SerializeField] CinemachineBlendDefinition.Style customBlend21;
    [Tooltip("camera2��camera1�B �u�����h�ɂ����鎞��"),SerializeField] float blendTime21;
    [Tooltip("�v���C���[���߂��Ă��čēx�ʉ߂����ۂɃJ������߂��@�\�����邩�ǂ���"),SerializeField] bool hasRecoverEffect;
    

    [Header("�J������؂�ւ����邩�ǂ���"), SerializeField] private bool canChangeCamera = true;

    public string CameraName1 { get => cameraName1; }
    public string CameraName2 { get => cameraName2; }

    private void Awake()
    {
        
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player") && canChangeCamera)
        {
            //���݂̃J�����̖��O��cameraName1�̏ꍇcameraName2�̃J�����ɐ؂�ւ���
            if (CameraManager.Instance?.GetCurrentCamera() == CameraManager.Instance?.GetCamera(cameraName1))
            {
                CameraManager.Instance?.ChangeCamera(cameraName2, customBlend12, blendTime12);
                
            }

            //���݂̃J�����̖��O��cameraName2�ŃJ������߂��@�\������ꍇ�ɂ�cameraName1�ɃJ������߂��B
            else if(CameraManager.Instance?.GetCurrentCamera() == CameraManager.Instance?.GetCamera(cameraName2) && hasRecoverEffect)
            {
                CameraManager.Instance?.ChangeCamera(cameraName1, customBlend21, blendTime21);
            }
        }
    }

    public void SetIfRecoverEffect(bool setRecover)
    {
        hasRecoverEffect = setRecover;
    }

    protected void SetIfCanChange(bool setCanChange)
    {
        canChangeCamera = setCanChange;
    }
}
