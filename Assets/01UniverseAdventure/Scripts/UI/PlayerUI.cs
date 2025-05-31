using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using DG.Tweening;

public class PlayerUI : MonoBehaviour
{
    [Tooltip("プレイヤーのhpを表示するライフゲージのセット"), SerializeField] GameObject lifeGauge;
    [Tooltip("プレイヤーのhpの値を示すライフゲージ"), SerializeField] Image lifeGauge1;
    [Tooltip("hpテキスト"), SerializeField] TextMeshProUGUI lifeText;
    [Tooltip("ストックを表示するテキスト"), SerializeField] TextMeshProUGUI stockText;
    [Tooltip("一つのライフバーで表せるhpの最大値"), SerializeField] float defaultMaxHp = 3;
    [Tooltip("プレイヤーの各パラメータを取得するためplayerStatusを取得しておく"), SerializeField] PlayerStatus playerStatus;
    [Tooltip("ライフ1の時ライフバーを振動させる時間"), SerializeField] float shakeDuration = 0.5f;
    [Tooltip("ライフバーを振動させる間隔"), SerializeField] float shakeInterval = 1f;
    [Tooltip("ライフバーの振動時の振幅"), SerializeField] float shakeStrength = 3f;
    Tweener shakeTween;
    Coroutine shakeCoroutine;


    private void Awake()
    {
        var initPos = lifeGauge1.rectTransform.position;
        playerStatus.Hp.Subscribe(hp => lifeGauge1.fillAmount = hp / defaultMaxHp).AddTo(gameObject);

        //hpが3の時はライフバーの色を緑、2の時は黄色、1の時は赤色にする
        playerStatus.Hp.Where(hp => hp == 3).Subscribe(_ => lifeGauge1.color = Color.green).AddTo(gameObject);

        playerStatus.Hp.Where(hp => hp == 2).Subscribe(_ =>
        {
            lifeGauge1.color = Color.yellow;
        }).AddTo(gameObject);

        playerStatus.Hp.Where(hp => hp == 1).Subscribe(_ =>
        {
            lifeGauge1.color = Color.red;
            shakeCoroutine = StartCoroutine(ShakeLifeGauge());//振動させる
        }).AddTo(gameObject);

        playerStatus.Hp.Where(hp => hp >= 2 && shakeCoroutine != null).Subscribe(_ =>
        {
            StopCoroutine(shakeCoroutine); //振動を止める
            shakeTween?.Kill();
            lifeGauge.transform.position = initPos;
        }).AddTo(gameObject);

        playerStatus.Hp.Subscribe(hp => lifeText.text = hp.ToString()).AddTo(gameObject); //hp表示

        playerStatus.Stock.Subscribe(stock => stockText.text = stock.ToString()).AddTo(gameObject); //stock表示
    }

    /// <summary>
    /// ライフバーを振動させるコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator ShakeLifeGauge()
    {
        while (playerStatus.Hp.Value == 1)
        {
            var lifeGaugeTransform = lifeGauge.transform;
            shakeTween = lifeGauge.transform.DOShakePosition(shakeDuration, shakeStrength); //DOTweenのDOShakePositionを使って振動させる
            yield return new WaitForSeconds(shakeInterval);
            shakeTween = null;
        }
    }
}
