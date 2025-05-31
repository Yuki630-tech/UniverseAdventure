using Cinemachine;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Splines;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerMover playerMover;
    [SerializeField] PlayerFlyToNextStage playerFlyToNextStage;
    [SerializeField] PlayerDrawnIntoBlackHole playerDrawnIntoBlackHole;
    [SerializeField] PlayerDamage playerDamage;
    [SerializeField] PlayerStatus playerStatus;
    [SerializeField] State currentState;
    [Tooltip("ブラックホールに吸い込まれるときにカメラをどれだけ後ろに下げるか"), SerializeField] float cameraDistance = 5f;
    [SerializeField] Rigidbody rb;
    [SerializeField] float cameraRotateSpeedWhenBlackHole = 1;
    [SerializeField] Animator animator;
    [SerializeField] PlayerCamera playerCamera;
    [SerializeField] Camera mainCamera;

    /// <summary>
    /// Stateの名前
    /// </summary>
    public enum State
    {
        Move,
        FlyToNextStage,
        BlackHole,
        Damage,
        Die
    }

    private void Awake()
    {
        //自身をGameManagerに登録→ゲームをリスタートした際にGameManagerがプレイヤーをチェックポイントまで移動させることができるようにする
        GameManager.Instance?.SetPlayer(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeToMoveState();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Move:
                playerMover.UpdateMove();
                break;
            case State.FlyToNextStage:
                playerFlyToNextStage.UpdateFly();
                break;
            case State.BlackHole:
                playerDrawnIntoBlackHole.UpdateGetDrawn();
                break;
            case State.Damage:
                //ダメージ状態ではなくなったときに、スターリングに乗った状態で通常移動の状態にならないように
                //FlyToNextステートの場合以外の時にMoveStateに移行するようにする。
                if (!playerDamage.IsDamaged && currentState != State.FlyToNextStage)
                {
                    ChangeToMoveState();
                    return;
                }
                break;
            case State.Die:

                break;
        }
    }
    
    /// <summary>
    /// MoveStateに移行させる関数。
    /// </summary>
    public void ChangeToMoveState()
    {
        //死亡した時、ブラックホールに吸い込まれている最中にMoveStateに移行しないようにする
        if (currentState == State.Die || currentState == State.BlackHole) return;
        ChangeToMoveStateTask();
        
    }

    public void ChangeToMoveStateTask()
    {
        if (playerMover != null)
        {
            playerMover.GoToMovable();
        }

        currentState = State.Move;
        rb.isKinematic = false;
        playerFlyToNextStage.SplineAnimate.enabled = false;
        //FlyToNextステートに変わる直前に記憶したベクトルが適用されてプレイヤーが勝手に移動することのないように
        //移動ベクトルをリセットする
        playerMover.InitializeVector();

        //カメラをスターリング用のカメラから通常のカメラに切り替える
        playerCamera.SetFlying(false);
    }

    public void ChangeToFlyState(SplineContainer spline)
    {
        if (playerMover != null)
        {
            playerMover.GoToImmovable();
        }
        //物理演算を停止
        rb.isKinematic = true;
        //スプラインコンポーネント有効化
        playerFlyToNextStage.StartFlyToNextStage(spline);
        currentState = State.FlyToNextStage;
        //カメラをスターリング用のカメラに切り替える。
        playerCamera.SetFlying(true);
    }

    public void ChangeToBlackHoleState(Transform setTransform)
    {
        //ブラックホールに吸い込まれている最中、死亡中、スターリングに乗っている間にブラックホールに吸い込まれることを防ぐ
        if (currentState == State.BlackHole || currentState == State.FlyToNextStage || currentState == State.Die) return;
        rb.isKinematic = true;
        //吸い込まれるブラックホールを登録
        playerDrawnIntoBlackHole.StartGetDrawn(setTransform);
        currentState = State.BlackHole;
        //----------------------カメラワーク----------------------------------------------
        playerMover.MainCamera.GetComponent<CinemachineBrain>().enabled = false;
        //プレイヤーが回転の中心からブラックホールの方向を向くように回転値を設定しその方向に向かせる
        var direction = setTransform.position - playerDrawnIntoBlackHole.Center;
        direction = direction.normalized;
        var look = Quaternion.LookRotation(direction);
        StartCoroutine(CameraManager.Instance?.RotateCameraTo(look, cameraRotateSpeedWhenBlackHole));

        //中心からcameraDistanceだけ後ろに下がった位置にカメラを移動→円運動を見やすくするため
        var position = playerDrawnIntoBlackHole.Center - playerDrawnIntoBlackHole.DirectionFromCenterToBlackHole * cameraDistance;
        StartCoroutine(CameraManager.Instance?.MoveCameraTo(position, cameraDistance));
    }

    /// <summary>
    /// ダメージを受ける関数→AttackReceiverのイベントとして登録
    /// </summary>
    /// <param name="data"></param>
    public void OnDamage(AttackData data)
    {
        //無敵状態、死亡状態、ブラックホールに吸い込まれている最中、スターリングに乗っている間、ゲームをポーズしている間にダメージを受けないようにする。
        bool isDisable = playerDamage.IsInvincible || currentState == State.Die || currentState == State.BlackHole || currentState == State.FlyToNextStage;
        if (isDisable || (GameManager.Instance != null &&  GameManager.Instance.IsPausing))
        {
            return;
        }
        currentState = State.Damage;
        playerDamage.OnTakeDamage(data);
        if (playerDamage.PlayerStatus.PlayerStatusData.Hp == 0)
        {
            OnDie();
        }
    }

    /// <summary>
    /// プレイヤーの死亡処理を行う関数。
    /// </summary>
    public void OnDie()
    {
        //死亡状態でさらに死亡してしまうことを防ぐ
        if(currentState == State.Die) { return; }   
        animator.SetTrigger("Die");
        playerDamage.StopFlash();
        //ブラックホールに吸い込まれている間にDieステートにすると回転が中断し落下してしまうのでブラックホールに吸い込まれている間はDieステートにならないようにする
        if(currentState != State.BlackHole)
        {
            currentState = State.Die;
        }
        if(playerStatus.Stock.Value > 0)
        {
            GameManager.Instance?.OnPlayerDie();

        }

        else
        {
            GameManager.Instance?.OnGameOver();
        }

    }

    /// <summary>
    /// ゲームがリスタートした際のプレイヤーの処理
    /// </summary>
    public void OnRestart()
    {
        playerStatus.RestartStatus();
        animator.ResetTrigger("Die");
        animator.SetTrigger("Restart");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            GameManager.Instance?.Goal();
        }
    }

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }
}
