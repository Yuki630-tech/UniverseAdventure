using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFadeOutObject : MonoBehaviour
{
    [Tooltip("�t�F�[�h�A�E�g�ɂ����鎞��"), SerializeField] float fadeOutTime = 1f;
    //�v���C���[���G�ꂽ�Ƃ��ɍ��Ȃ��Ă���Audio��fadeout������
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance?.FadeOut(fadeOutTime);
        }
    }
}
