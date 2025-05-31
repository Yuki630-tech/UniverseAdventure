using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System;

public class AudioChangeGravitySEPlayer : MonoBehaviour
{
    [Tooltip("重力切り替え時の警告を鳴らすAudioSource"),SerializeField] AudioSource audioSource;
    [Tooltip("重力が切り替わることを警告するAudioClip"),SerializeField] AudioClip warningAudioClip;
    [Tooltip("重力が切り替わる時に鳴らすAudioClip"), SerializeField] AudioClip gravityChangeAudioClip;

    /// <summary>
    /// 警告音を鳴らす関数
    /// </summary>
    /// <returns></returns>

    public void PlayWarningGravitySE()
    {
        audioSource.PlayOneShot(warningAudioClip);
    }

    /// <summary>
    /// 重力が切り替わった時のSEを鳴らす関数
    /// </summary>

    public void PlayChangeGravitySE()
    {
        audioSource.PlayOneShot(gravityChangeAudioClip);
    }
}
