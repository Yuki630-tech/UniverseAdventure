using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFadeOutObject : MonoBehaviour
{
    [Tooltip("フェードアウトにかける時間"), SerializeField] float fadeOutTime = 1f;
    //プレイヤーが触れたときに今なっているAudioをfadeoutさせる
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance?.FadeOut(fadeOutTime);
        }
    }
}
