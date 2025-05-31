using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInImage:MonoBehaviour
{
    [Tooltip("fadein������Image"), SerializeField] Image fadeInImage;

    [Tooltip("fadein�ɂ����鎞��"), SerializeField] float fadeInTime;
    /// <summary>
    /// �C���[�W��fadein������R���[�`���B
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeInCoroutine()
    {
        
        fadeInImage.gameObject.SetActive(true);
        float currentTime = 0f;
        float rate = 0f;
        //currentTime��fadeInTime�ȏ�ɂȂ�܂�
        while (currentTime <= fadeInTime)
        {
            //currentTime�����Z����fadeInTime�ɑ΂���currentTime�̊���rate��fadein������Image�̃��l�ɐݒ�
            currentTime += Time.deltaTime;
            rate = currentTime / fadeInTime;

            rate = Mathf.Clamp01(rate);
            var color = fadeInImage.color;
            color.a = rate;
            fadeInImage.color = color;
            yield return null;
        }
    }
}
