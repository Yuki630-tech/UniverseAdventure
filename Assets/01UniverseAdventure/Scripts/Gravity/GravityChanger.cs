using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class GravityChanger : MonoBehaviour
{
    ReactiveProperty<float> gravityDirection = new ReactiveProperty<float>();
    [SerializeField] AudioChangeGravitySEPlayer audioChangeGravitySEPlayer;
    [SerializeField] CollectQuest collectQuest;
    [Tooltip("警告を通知する間隔"), SerializeField] float warningInterval = 3.5f;
    [Tooltip("ポーズを解除してから警告を通知するまでの間隔"), SerializeField] float warningIntervalFromUnPause = 1.3f;
    [Tooltip("警告音を鳴らす間隔"), SerializeField] float warningSEPlayInterval = 0.5f;
    int warningCount = 3; //１度の警告で音を鳴らす回数
    IDisposable playerDieDisposable;
    IDisposable gameClearDisposable;
    [SerializeField] PauseUI pauseUI;
    [Header("Pauseした後再開した際に重力切り替えも再開させるかどうか"), SerializeField] bool isResumableAfterPause;
    [Header("コルーチン再開用のフラグ"), SerializeField] bool isNotPausing;
    /// <summary>
    /// 重力切り替えのコルーチン。停止できるよう変数に保持しておく
    /// </summary>
    public Coroutine GravityChangeCoroutine { get; private set; }

    /// <summary>
    /// 一定時間ごとに+1/-1と切り替わる重力の向きのプロパティ。+1だとプレイヤーの上方向が初期値の方向に,-1では逆の方向に向く。
    /// </summary>
    public IReadOnlyReactiveProperty<float> GravityDirection => gravityDirection;

    private void Awake()
    {
        isNotPausing = true;
        isResumableAfterPause = true;
        //クエストノルマを達成したときに流れる演出の開始時に重力切り替えを止め、プレイヤーが再び動けるようになったら再開するようにするように
        collectQuest.OnStopQuestObservable.Subscribe(_ => StopGravityChange()).AddTo(gameObject);
        collectQuest.OnQuitQuestObservable.Subscribe(_ =>
        {
            StopGravityChange();
            isResumableAfterPause = false;
        }).AddTo(gameObject);
        collectQuest.OnReplayQuestObservable.Subscribe(_ => RestartGravityChange()).AddTo(gameObject);


    }

    /// <summary>
    /// 一定時間ごとの重力切り替えを開始する関数(SetGravityChangerオブジェクトにプレイヤーが触れたときのUnityEventとして登録)
    /// </summary>
    public void StartGravityChange()
    {
        //最初は上方向にプレイヤーが向くように
        gravityDirection.Value = 1;
        GravityChangeCoroutine = StartCoroutine(GravityChange());
        //クリア、プレイヤー死亡時に重力切り替えを停止するようにする
        playerDieDisposable = GameManager.Instance?.OnPlayerDieObservable.Subscribe(_ => StopGravityChange()).AddTo(this);
        gameClearDisposable = GameManager.Instance?.OnGameClearOrOverObservable.Subscribe(_ => StopGravityChange()).AddTo(this);
        GameManager.Instance?.OnPauseGameObservable.Subscribe(_ => isNotPausing = false).AddTo(this);
        GameManager.Instance?.OnUnPauseGameObservable.Where(_ => isResumableAfterPause).Subscribe(_ => isNotPausing = true).AddTo(this);
    }

    /// <summary>
    /// クエストノルマ達成後の演出が終了した時に一定時間ごとの重力切り替えを再開する関数
    /// </summary>
    public void RestartGravityChange()
    {
        Debug.Log("重力切り替え再開");
        GravityChangeCoroutine = StartCoroutine(GravityChange());
        playerDieDisposable = GameManager.Instance?.OnPlayerDieObservable.Subscribe(_ => StopGravityChange()).AddTo(this);
        gameClearDisposable = GameManager.Instance?.OnGameClearOrOverObservable.Subscribe(_ => StopGravityChange()).AddTo(this);
    }

    /// <summary>
    /// 一定時間ごとの重力切り替えを停止する関数
    /// </summary>
    public void StopGravityChange()
    {
        if (GravityChangeCoroutine != null)
        {
            StopCoroutine(GravityChangeCoroutine);
            GravityChangeCoroutine = null;
            playerDieDisposable.Dispose();
            gameClearDisposable.Dispose();
        }


    }
    /// <summary>
    /// 一定時間ごとに重力を切り替えるコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator GravityChange()
    {
    
        while (true)
        {
            yield return new WaitUntil(() => isNotPausing);
            yield return new WaitForSeconds(warningInterval);
            if (!isNotPausing)
            {
                yield return new WaitUntil(() => isNotPausing);
                yield return new WaitForSeconds(warningIntervalFromUnPause);
            }
            for (int i = 0; i < warningCount; i++)
            {
                audioChangeGravitySEPlayer.PlayWarningGravitySE();
                yield return new WaitForSeconds(warningSEPlayInterval);
            }
            //リアクティブプロパティの値を-
            gravityDirection.Value = -gravityDirection.Value;
            audioChangeGravitySEPlayer.PlayChangeGravitySE();
        }
    }
}
