using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class GoalScene : MonoBehaviour
{
    [Tooltip("タイトルシーンのボタン"), SerializeField] Button titleButton;
    [Tooltip("fadeinさせる背景画像"), SerializeField] FadeInImage fadeInImage;
    [Tooltip("クリアシーンで表示させるテキストとボタンのセット"), SerializeField] GameObject goalTextAndButton;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        titleButton.OnClickAsObservable().Subscribe(_ => SceneManager.LoadScene("TitleScene")).AddTo(this);
        //最初はUI非表示
        goalTextAndButton.SetActive(false);
        fadeInImage.gameObject.SetActive(false);
        StartCoroutine(GoalSceneGoroutine());
    }

    /// <summary>
    /// ゴールシーンを表示させるコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator GoalSceneGoroutine()
    {
        
        fadeInImage.gameObject.SetActive(true);
        yield return fadeInImage.FadeInCoroutine();
        goalTextAndButton.SetActive(true);
        GameManager.Instance?.OnGoalSceneCompleted();
    }
}
