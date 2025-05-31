using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using Cysharp.Threading.Tasks;

public class DieScene : MonoBehaviour
{
    [Tooltip("gameOverのテキスト"), SerializeField] GameObject gameOverText;
    PlayerStatus playerStatus; //ストックを表示させるためにプレイヤーのステータス情報を取得
    [Tooltip("プレイヤーが死んでからgameOverテキストを表示させるまでの時間"), SerializeField] float waitTimeFromDieToAppearText = 1;
    [Tooltip("gameOverテキストが表示されてからフェイドアウトするまでの時間"), SerializeField] float waitTimeUntilDieTextFade = 1;
    [Tooltip("gameOverテキストが非表示になってからストックテキストが表示されるまでの時間"), SerializeField] float waitTimeFromDieTextToStockText = 1f;
    [Tooltip("ストックテキストが表示されてからストックを減らすまでの時間"), SerializeField] float waitTimeFromStockTextToRemoveStock = 1f;
    [Tooltip("小待機の時間(テキストが表示されてからブラックアウトを開始するまでなど短い待機)"), SerializeField] float waitTimeFromAppearTextToNextEvent = 0.5f;
    [Tooltip("ストックを減らしてからゲームがリスタートするまでの時間"), SerializeField] float waitTimeToRestart = 1;
    [Tooltip("ストックUI"), SerializeField] GameObject stockUISet;
    [Tooltip("ストックテキスト"), SerializeField] TextMeshProUGUI stockText;
    [Tooltip("フェイドインさせる背景"), SerializeField] FadeInImage blackBackGround;

    private void Awake()
    {
        playerStatus = FindObjectOfType<PlayerStatus>();
        //最初は非表示
        gameOverText.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        //ストックをstockTextに表示させる。
        playerStatus.Stock.Subscribe(stock => stockText.text = stock.ToString()).AddTo(this);

        StartCoroutine(DieSceneCoroutine());
    }

    /// <summary>
    /// diesceneのコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator DieSceneCoroutine()
    {
        var appearDieTextWait = new WaitForSeconds(waitTimeFromDieToAppearText);
        var fadeDieTextWait = new WaitForSeconds(waitTimeUntilDieTextFade);
        var stockTextAppearWait = new WaitForSeconds(waitTimeFromDieTextToStockText);
        var stockRemoveWait = new WaitForSeconds(waitTimeFromStockTextToRemoveStock);
        var smallWait = new WaitForSeconds(waitTimeFromAppearTextToNextEvent);
        var restartWait = new WaitForSeconds(waitTimeToRestart);

        AudioManager.Instance?.StopAudio();
        AudioManager.Instance?.PlaySE(AudioClipData.SeAudioClipName.Die);
        yield return appearDieTextWait; //テキスト表示を待機
        gameOverText.SetActive(true); //テキスト表示
        AudioManager.Instance?.PlayAudio(AudioClipData.AudioClipName.DieBGM); //死亡bgm再生
        yield return smallWait; //少し待機
        yield return blackBackGround.FadeInCoroutine(); //ブラックアウト
        yield return fadeDieTextWait; //テキスト非表示を待機
        gameOverText.SetActive(false); //テキスト非表示
        yield return stockTextAppearWait; //ストックUI表示待機
        stockUISet.SetActive(true); //ストックUI
        yield return stockRemoveWait; //ストックを減らすまで待機
        playerStatus.ReduceStock();　//ストックを減らす
        yield return restartWait; //リスタート待機
        GameManager.Instance?.RestartGame(); //リスタート

    }

   

    
}
