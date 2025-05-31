using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using System;
using Cysharp.Threading.Tasks;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    [Tooltip("プレイヤーが死亡した時に表示させるUIのプレハブ"), SerializeField] GameObject playerDieCanvasPrefab;
    [Tooltip("ゴールした際に表示させるUIのプレハブ"), SerializeField] GameObject goalCanvasPrefab;
    [Tooltip("ゲームオーバーした際に表示させるUIのプレハブ"), SerializeField] GameObject gameOverCanvasPrefab;

    [Header("シーン上にいるプレイヤー"), SerializeField] Player player;
    [Header("死亡した後にプレイヤーがリスポーンする場所の座標"), SerializeField] Vector3 restartPos;
    [Header("死亡した際に表示されたUI"), SerializeField] GameObject gameOverCanvas;
    [Header("ゴールした時に表示されたUI"), SerializeField] GameObject goalCanvas;

    //サブジェクトのセット
    Subject<Unit> onPlayerDieSubject = new Subject<Unit>();
    Subject<Unit> onPlayerRestartSubject = new Subject<Unit>();
    Subject<Unit> onGameClearOrOverSubject = new Subject<Unit>();
    Subject<Unit> onResetGameSubject = new Subject<Unit>();
    Subject<Unit> onPauseGameSubject = new Subject<Unit>();
    Subject<Unit> onUnPauseGameSubject = new Subject<Unit>();


    /// <summary>
    /// プレイヤーが死んだときの処理を登録するためのIObservable
    /// </summary>
    public IObservable<Unit> OnPlayerDieObservable => onPlayerDieSubject;

    /// <summary>
    /// ゲームがリスタートした時の処理を登録するためのIObservable
    /// </summary>
    public IObservable<Unit> OnPlayerRestartObservable => onPlayerRestartSubject;

    /// <summary>
    /// ゲームクリア後再プレイした時の処理を登録するためのIObservable
    /// </summary>
    public IObservable<Unit> OnResetGameObservable => onResetGameSubject;

    /// <summary>
    /// ゲームをクリアした時の処理を登録するためのIObservable
    /// </summary>
    public IObservable<Unit> OnGameClearOrOverObservable => onGameClearOrOverSubject;

    /// <summary>
    /// ゲームをポーズした時の処理を登録するためのIObservable
    /// </summary>
    public IObservable<Unit> OnPauseGameObservable => onPauseGameSubject;

    /// <summary>
    /// ゲームがアンポーズした時の処理を登録するためのIObservable
    /// </summary>
    public IObservable<Unit> OnUnPauseGameObservable => onUnPauseGameSubject;
    
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// 初期プレイかどうか(初期プレイ以外の際にはタイトル画面の演出をスキップできるようにする)
    /// </summary>
    public bool IsFirstPlay { get; private set; }

    /// <summary>
    /// ゲームがポーズされた状態かどうか
    /// </summary>
    public bool IsPausing {  get; private set; }

    private void Awake()
    {
        //シングルトン
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }

        //subjectがゲームオブジェクトの破棄とともに破棄されるようにする
        onPlayerDieSubject.AddTo(this);
        onPlayerRestartSubject.AddTo(this);
        onGameClearOrOverSubject.AddTo(this);
        onPauseGameSubject.AddTo(this);
        onUnPauseGameSubject.AddTo(this);
        IsFirstPlay = true;
        IsPausing = false;

    }

    /// <summary>
    /// ゲームをスタートさせる関数
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
        IsFirstPlay = false;
    }

    /// <summary>
    /// ゲームをやめる関数
    /// </summary>
    public void QuitGame()
    {
#if　UNITY_EDITOR
        EditorApplication.isPlaying = false;

#elif PLATFORM_WEBGL
        SceneManager.LoadScene("TitleScene");

#else
        Application.Quit();
#endif
    }



    /// <summary>
    /// gameOverしたあとにゲームリセットを通知する関数(プレイヤーのストックを初期値に戻す処理など)
    /// </summary>
    public void ResetGame()
    {
        onResetGameSubject.OnNext(Unit.Default);
    }

    /// <summary>
    /// プレイヤーを登録する関数
    /// </summary>
    /// <param name="setPlayer"></param>
    public void SetPlayer(Player setPlayer)
    {
        player = setPlayer;
    }

    /// <summary>
    /// リスポーンする先の座標を登録する関数
    /// </summary>
    /// <param name="setPos"></param>

    public void SetRestartPos(Vector3 setPos)
    {
        restartPos = setPos;
    }

    /// <summary>
    /// ゲームのリスタート処理
    /// </summary>

    public void RestartGame()
    {
        //プレイヤーをチェックポイントに戻してhpを初期値に戻す処理をする
        player.transform.position = restartPos;
        player.OnRestart();
        if(player.transform.position == restartPos)
        {
            player.ChangeToMoveStateTask();
        }

        //リスタートしたことを通知する
        onPlayerRestartSubject.OnNext(Unit.Default);
        Destroy(gameOverCanvas);
        
    }

    /// <summary>
    /// プレイヤーが死亡した時の処理
    /// </summary>
    public void OnPlayerDie()
    {
        //プレイヤーが死んだことを通知する
        onPlayerDieSubject.OnNext(Unit.Default);

        //死亡シーンの表示
        gameOverCanvas = Instantiate(playerDieCanvasPrefab);
    }

    /// <summary>
    /// プレイヤーがゴールした時の処理
    /// </summary>

    public void Goal()
    {
        //ゴールシーン表示
        goalCanvas = Instantiate(goalCanvasPrefab);
        //ゲームクリアしたことを通知する
        onGameClearOrOverSubject.OnNext(Unit.Default);
    }

    /// <summary>
    /// ゴールシーンの一連の流れが終了した時の処理
    /// </summary>
    public void OnGoalSceneCompleted()
    {
        //プレイヤーを非表示に→プレイヤーにかかる諸々の処理を終了させるため
        player.gameObject.SetActive(false);
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>

    public void OnGameOver()
    {
        DebugLog.Log("ゲームオーバーです");
        //ゲームオーバーシーンを表示
        gameOverCanvas = Instantiate(gameOverCanvasPrefab);
        //ゲームオーバーしたことを通知する
        onGameClearOrOverSubject.OnNext(Unit.Default);
    }
    /// <summary>
    /// ゲームをポーズする
    /// </summary>
    public void OnPauseGame()
    {
        //ゲームがポーズされたことを通知する
        onPauseGameSubject.OnNext(Unit.Default);

        //ポーズモードに
        IsPausing = true;
    }

    /// <summary>
    /// ゲームをアンポーズする
    /// </summary>
    public void OnUnPauseGame()
    {
        //ゲームがアンポーズされたことを通知する
        onUnPauseGameSubject.OnNext(Unit.Default);

        //ポーズモード解除
        IsPausing = false;
    }
}
