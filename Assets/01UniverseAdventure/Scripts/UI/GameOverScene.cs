using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScene : MonoBehaviour
{
    [Tooltip("ゲームオーバー時に表示させるテキストとボタンのセット"), SerializeField] GameObject gameOverTextAndButton;
    [Tooltip("ゲームオーバーボタン"), SerializeField] Button gameOverButton;
    [Tooltip("fadeinさせる背景"), SerializeField] FadeInImage fadeInImage;
    [Tooltip("死亡してから死亡ジングルを鳴らすまでの時間"), SerializeField] float intervalUntilAudioPlay;
    private void OnEnable()
    {
        //最初はすべてのUI非表示
        gameOverTextAndButton.SetActive(false);
        fadeInImage.gameObject.SetActive(false);
        //ボタンを押したらTitleSceneに戻るようにする。またゲームをリセット
        gameOverButton.OnClickAsObservable().Subscribe(_ =>
        {
            SceneManager.LoadScene("TitleScene");
            GameManager.Instance?.ResetGame();
        }).AddTo(gameObject);
        StartCoroutine(GameOverCoroutine());
    }

    /// <summary>
    /// ゲームオーバーシーンのコルーチン。効果音→待機→死亡bgm→背景fadein→テキスト、ボタン表示
    /// </summary>
    /// <returns></returns>
    IEnumerator GameOverCoroutine()
    {
        if (AudioManager.Instance == null) yield break;
        var waitForPlayAudio = new WaitForSeconds(intervalUntilAudioPlay);
        var waitWhileAudioPlaying = new WaitWhile(() => AudioManager.Instance.IsAudioPlaying());
        AudioManager.Instance.StopAudio(); //無音の状態にする
        AudioManager.Instance.PlaySE(AudioClipData.SeAudioClipName.Die); //死亡のSE
        yield return waitForPlayAudio;
        AudioManager.Instance.PlayAudio(AudioClipData.AudioClipName.GameOverBGM);
        yield return fadeInImage.FadeInCoroutine();
        yield return waitWhileAudioPlaying;
        gameOverTextAndButton.SetActive(true);
        AudioManager.Instance.PlayAudio(AudioClipData.AudioClipName.TitleBGM);


    }
}
