using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System;

public class AudioChangeGravitySEPlayer : MonoBehaviour
{
    [Tooltip("�d�͐؂�ւ����̌x����炷AudioSource"),SerializeField] AudioSource audioSource;
    [Tooltip("�d�͂��؂�ւ�邱�Ƃ��x������AudioClip"),SerializeField] AudioClip warningAudioClip;
    [Tooltip("�d�͂��؂�ւ�鎞�ɖ炷AudioClip"), SerializeField] AudioClip gravityChangeAudioClip;

    /// <summary>
    /// �x������炷�֐�
    /// </summary>
    /// <returns></returns>

    public void PlayWarningGravitySE()
    {
        audioSource.PlayOneShot(warningAudioClip);
    }

    /// <summary>
    /// �d�͂��؂�ւ��������SE��炷�֐�
    /// </summary>

    public void PlayChangeGravitySE()
    {
        audioSource.PlayOneShot(gravityChangeAudioClip);
    }
}
