using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class CollectQuest : MonoBehaviour
{
    [Tooltip("このクエストで集めるアイテムに設定するタグの名前(ステージ名(このコンポーネントがアタッチされているオブジェクト名)"), SerializeField] 
    string collectObjTagName;
    [Tooltip("このクエストをスタートするためにプレイヤーが触れるオブジェクトに設定するタグの名前(ステージ名 + Start)"), SerializeField] string startObjTagName;
    [Tooltip("このクエストが完全に終了するために触れるオブジェクトの名前"), SerializeField] string quitObjTagName; 
    [Tooltip("集めるべき個数"), SerializeField] int collectNum;
    [Tooltip("次のステージへの移動手段(クエスト中は非表示、完了時に表示させるオブジェクト)"), SerializeField] GameObject[] leadToNextStageObjs;
    [Tooltip("移動手段を出現させる際に鳴らすSEの種類"), SerializeField] List<AudioClipData.SeAudioClipName> seAudioClipNames;
    [Tooltip("表示させるオブジェクトが2つある際に1つ前のオブジェクトを表示させてから次をカメラで映し出すまでの時間。" +
        "1つの場合、また最後のオブジェクトについては0に設定する"), SerializeField]
    float[] intervals;
    [Tooltip("プレイヤーのオブジェクト。クエスト開始、終了を感知するために取得。"), SerializeField] GameObject player;

    [Header("現在集めた個数"), SerializeField] ReactiveProperty<int> currentCollectNum;
    [Tooltip("映し出すカメラの名前(映す順番に登録すること)"), SerializeField] string[] cameraName;
    [Tooltip("プレイヤーを映し出すカメラの名前"), SerializeField] string playerCamera;
    [Tooltip("カメラで映し出してからオブジェクトを表示させるまでの時間"), SerializeField] float activeInterval;
    [Tooltip("最後のオブジェクトを映してからプレイヤーを映すまでの時間"), SerializeField] float returnCameraPriorityInterval;
    [Tooltip("プレイヤーを映してすぐに無敵状態を解除すると難易度が高くなるのでしばらく待つ。その待機時間"), SerializeField] float invincibleInterval = 1f;
    [Tooltip("集めた個数を表示させるUI"), SerializeField] GameObject collectUI;
    [Tooltip("集めた個数を表示させるテキスト"), SerializeField] TextMeshProUGUI collectNumText;

    [Tooltip("クエストが完了した際に表示させるオブジェクト"), SerializeField] ObjectActivatedAfterQuestClear objectActivatedAfterQuestClear;

    IDisposable triggerDisposable;
    List<GameObject> collectedObj = new List<GameObject>(); //プレイヤーが死亡してリスタートした時に集めて非表示になったアイテムをもとに戻すことができるように、
                                                            //集めたアイテムを保持しておくためのリスト
    //クエストに関係するsubject群
    Subject<Unit> onCompleteQuestSubject = new Subject<Unit>();
    Subject<Unit> onStopQuestSubject = new Subject<Unit>();
    Subject<Unit> onReplayQuestSubject = new Subject<Unit>();
    Subject<Unit> onQuitQuestSubject = new Subject<Unit>();

    /// <summary>
    /// クエストが完了したことを感知するIObservable
    /// </summary>
    public IObservable<Unit> OnCompleteQuestObservable => onCompleteQuestSubject;

    /// <summary>
    /// クエストが一時中断したことを感知するIObservable
    /// </summary>
    public IObservable<Unit> OnStopQuestObservable => onStopQuestSubject;

    /// <summary>
    /// クエストがリプレイされたことを感知するIObservable
    /// </summary>
    public IObservable<Unit> OnReplayQuestObservable => onReplayQuestSubject;

    /// <summary>
    /// クエストが完全に完了したことを感知するIObservable
    /// </summary>
    public IObservable<Unit> OnQuitQuestObservable => onQuitQuestSubject;
    private void Awake()
    {
        objectActivatedAfterQuestClear?.ChangeActiveState(false);
        //クエストをスタートさせるオブジェクトにプレイヤーが触れたときクエストを開始するようにする。
        player.OnTriggerEnterAsObservable().Where(collider => collider.CompareTag(startObjTagName)).Subscribe(_ => StartQuest()).AddTo(this);

        //プレイヤーが次のステージ進んだ時にクエストを止める。(重力切り替えをストップするなど)
        player.OnTriggerEnterAsObservable().Where(collider => quitObjTagName.Length != 0 && collider.CompareTag(quitObjTagName)).Subscribe(_ => onQuitQuestSubject.OnNext(Unit.Default)).AddTo(this);
        onCompleteQuestSubject.AddTo(this);
        onStopQuestSubject.AddTo(this);
        onReplayQuestSubject.AddTo(this);
        collectUI.SetActive(false);
        GameManager.Instance.OnPlayerRestartObservable.Where(_ => collectedObj.Count > 0).Subscribe(_ =>
        {
            //リスタート時にすでに取得済みのアイテムがあれば再表示する
            foreach (var obj in collectedObj)
            {
                obj.SetActive(true);
            }

            collectedObj.Clear();
        });

        DeactivateLeadObject();

        GameManager.Instance.OnPlayerRestartObservable.Subscribe(_ =>
        {
            DeactivateLeadObject();
        });
    }

    /// <summary>
    /// クエストをスタートさせる関数
    /// </summary>
    public void StartQuest()
    {
        collectUI.SetActive(true);
        //アイテムに触れたときの購読を破棄する
        triggerDisposable?.Dispose();

        //集めた数を初期化する
        currentCollectNum.Value = 0;
        

        //集めた数と集めるべき数をUI表示
        currentCollectNum.Subscribe(num => collectNumText.text = num.ToString() + " / " + collectNum.ToString()).AddTo(gameObject);

        //アイテムに触れたときにアイテムを非表示にして収集数を一つ増やす。また、破棄できるようにiDisposableに購読者を登録しておく。
        triggerDisposable = player.OnTriggerEnterAsObservable().Where(collider => collider.CompareTag(collectObjTagName)).Subscribe(collider => 
        {
            collider.transform.parent.gameObject.SetActive(false);
            collectedObj.Add(collider.transform.parent.gameObject);
            AddCollectNum();
            AudioManager.Instance?.PlaySE(AudioClipData.SeAudioClipName.GetItem);
        }).AddTo(gameObject);

    }

    /// <summary>
    /// 収集数を増やす関数
    /// </summary>
    public void AddCollectNum()
    {
        currentCollectNum.Value++;
        //収集数がノルマを達成したらクエストクリアとする
        if(currentCollectNum.Value >= collectNum)
        {
            CompleteQuest();
        }
    }

    /// <summary>
    /// クエストクリアの関数
    /// </summary>
    [ContextMenu("PlayEndEvent")]
    
    void CompleteQuest()
    {
        objectActivatedAfterQuestClear?.ChangeActiveState(true);
        collectUI.SetActive(false);
        StartCoroutine(CompleteQuestCoroutine());
        currentCollectNum.Value = 0;

    }

    /// <summary>
    /// 次のステージへの移動手段をすべて非表示
    /// </summary>

    void DeactivateLeadObject()
    {
        foreach (var obj in leadToNextStageObjs)
        {
            obj.SetActive(false);

        }
    }

    /// <summary>
    /// クエストクリアのコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator CompleteQuestCoroutine()
    {
        var pauseUI = FindObjectOfType<PauseUI>();
        pauseUI.ChangeIfEnableToPause(false);
        var playerMover = player.GetComponent<PlayerMover>(); //演出中、勝手にプレイヤーが動かないようにするためにplayerMoverを取得して移動できなくする
        var playerDamage = player.GetComponent<PlayerDamage>(); //演出中、プレイヤーがダメージを受けないようにするためにplayeDamageを取得してダメージを受けないようにする。
        onStopQuestSubject.OnNext(Unit.Default); //一時停止を通知
        playerDamage.ChangeInvicibleValue(true);
        for (int i = 0; i < leadToNextStageObjs.Length; i++)
        {
            int index = i;

            //カメラ処理
            CameraManager.Instance?.ChangeCamera(cameraName[index]);
            playerMover.GoToImmovable(); //動けないようにする
            yield return new WaitForSeconds(activeInterval);
            //移動手段表示
            leadToNextStageObjs[i].SetActive(true);
            AudioManager.Instance?.PlaySE(seAudioClipNames[index]);
            yield return new WaitForSeconds(intervals[index]);
        }
        
        yield return new WaitForSeconds(returnCameraPriorityInterval);
        playerMover.GoToMovable(); //動けるようにする
        //カメラ処理
        CameraManager.Instance?.ChangeCamera(playerCamera);
        pauseUI.ChangeIfEnableToPause(true);
        onReplayQuestSubject.OnNext(Unit.Default); //クエストリプレイの通知
        onCompleteQuestSubject.OnNext(Unit.Default); //クエストクリアの通知
        yield return new WaitForSeconds(invincibleInterval);
        playerDamage.ChangeInvicibleValue(false); //無敵状態解除
    }
}
