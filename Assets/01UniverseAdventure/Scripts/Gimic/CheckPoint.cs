using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    bool isEffective;
    private void Awake()
    {
        isEffective = true;
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isEffective)
        {
            
            //�Q�[�������X�^�[�g�����Ƃ��ɂ��̏ꏊ�ɖ߂��Ă���悤�ɂ��̃I�u�W�F�N�g�̍��W�����X�|�[���n�ɐݒ肷��
            GameManager.Instance.SetRestartPos(transform.position);

            //�Q�[�������X�^�[�g��������bgm�ƃJ�����ɂ��ꂼ�ꌻ�ݗL���ɂȂ��Ă�����̂�o�^���Ă���
            AudioManager.Instance.SetAudioBeforeDie();
            CameraManager.Instance.SetCameraBeforeDie();

            CameraManager.Instance.SetMainCamera();
            isEffective = false;
        }
    }
}
