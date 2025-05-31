using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInImage:MonoBehaviour
{
    [Tooltip("fadeinさせるImage"), SerializeField] Image fadeInImage;

    [Tooltip("fadeinにかける時間"), SerializeField] float fadeInTime;
    /// <summary>
    /// イメージをfadeinさせるコルーチン。
    /// </summary>
    /// <returns></returns>
    public IEnumerator FadeInCoroutine()
    {
        
        fadeInImage.gameObject.SetActive(true);
        float currentTime = 0f;
        float rate = 0f;
        //currentTimeがfadeInTime以上になるまで
        while (currentTime <= fadeInTime)
        {
            //currentTimeを加算させfadeInTimeに対するcurrentTimeの割合rateをfadeinさせるImageのα値に設定
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
