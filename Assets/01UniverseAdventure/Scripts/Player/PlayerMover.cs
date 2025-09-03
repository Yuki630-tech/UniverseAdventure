using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Cinemachine.Utility;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;
using Cinemachine;

public class PlayerMover : Movable
{
    [Tooltip("入力を制御するマネージャー"), SerializeField] InputManager inputManager;
    [Tooltip("足音を鳴らすオーディオソース"), SerializeField] AudioSource audioSource;
    [Header("どのレイヤーでどの足音を流すか"), SerializeField] private List<FootSeSet> footSeSets = new List<FootSeSet>();
    [Tooltip("足音のオーディオクリップ"), SerializeField] AudioClip footAudioClip;
    [Tooltip("ジャンプ時のオーディオクリップ"), SerializeField] AudioClip jumpAudioclip;
    [Tooltip("踏みつけ時のオーディオクリップ"), SerializeField] AudioClip stepOnAudioClip;
    [Tooltip("移動速度"), SerializeField] float moveSpeed;
    [Tooltip("接地判定を行うコンポーネント"), SerializeField] GroundChecker groundChecker;
    [Tooltip("ジャンプ力"), SerializeField] float jumpPower;
    [Tooltip("攻撃時のジャンプ力"), SerializeField] float attackJumpPower;
    [Tooltip("プレイヤーのアニメーターコンポーネント"), SerializeField] Animator animator;

    [Tooltip("移動手段として用いる物理挙動"), SerializeField] Rigidbody rb;
    [Tooltip("プレイヤーの重力を操作するコンポーネント"), SerializeField] Gravity gravity;
    [Tooltip("シーン上のメインカメラ"), SerializeField] Camera mainCamera;
    [Tooltip("接地状態かどうか"), SerializeField] bool isGround;
    [Tooltip("カメラとプレイヤーの上方向とを垂直とみなす内積"), SerializeField] float verticalDot = 0f;
    [Tooltip("プレイヤーを映すvirtualcamera"), SerializeField] CinemachineVirtualCamera playerVirtualCamera;
    CinemachinePOV pov;
    Vector3 moveDirection;
    bool isMovable;
    Vector3 basedVector;
    Vector3 right;
    Vector3 forward;
    [Tooltip("プレイヤーの上にカメラがあるかどうか"), SerializeField] bool isUp;
    bool isUpSideDown;
    [Tooltip("平行状態とみなす内積の最大値"), SerializeField] float parallelThreshould = 0.8f;
    [Tooltip("上下さかさまであるとみなす内積の最大値"), SerializeField] float upsideDownThreshould = 0.4f;
    ReactiveProperty<Vector3> gravityNormalProperty = new ReactiveProperty<Vector3>();
    bool planetIsSmallBox;
    bool isOnlyMoveToSide;
    bool isEnableToRotateTowardCamera;

    [SerializeField] ReactiveProperty<float> cameraRotProperty = new ReactiveProperty<float>();
    public override bool IsGround { get => isGround; }
    public Camera MainCamera { get => mainCamera; }
    public bool IsOnlyMoveToSide { get => isOnlyMoveToSide; }

    [Serializable]
    private struct FootSeSet
    {
        public LayerMask groundLayer;
        public AudioClip footSe;
    }

    // Start is called before the first frame update
    void Start()
    {
        gravityNormalProperty.Pairwise().Where(pair => Vector3.Angle(pair.Previous, pair.Current) == 90).Subscribe(async async => {
            isEnableToRotateTowardCamera = false;
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            isEnableToRotateTowardCamera = true;
        });
        pov = playerVirtualCamera.GetCinemachineComponent<CinemachinePOV>();
        isMovable = true;
        isEnableToRotateTowardCamera = true;
        //キーボード、コントローラーの操作の入力が変化した際に動く方向を設定する。
        inputManager.DirectionInput.Where(_ => isMovable && !isOnlyMoveToSide).Subscribe(input => GetAdjustedVector(input)).AddTo(this);

        //横スクロール
        inputManager.DirectionInput.Where(_ => isMovable && isOnlyMoveToSide).Subscribe(input => GetAdjustedVector(new Vector3(input.x, 0f, 0f))).AddTo(this);

        //カメラに合わせて回転
        cameraRotProperty.Where(_ => isMovable && !isOnlyMoveToSide && isEnableToRotateTowardCamera)
            .Where(_ => gravity.Planet == null || gravity.Planet.PlanetTypeParam != Planet.PlanetType.SmallBox)
            .Subscribe(_ =>
        {
            GetAdjustedVector(inputManager.DirectionInput.Value);
        });

        //ゲームがポーズした時にプレイヤーが動かないようにする
        GameManager.Instance?.OnPauseGameObservable.Subscribe(_ => GoToImmovable()).AddTo(gameObject);

        //ゲームがポーズからリスタートした時にプレイヤーが動けるようにする
        GameManager.Instance?.OnUnPauseGameObservable.Subscribe(_ => GoToMovable()).AddTo(gameObject);

        //プレイヤーが死んでゲームがリスタートした時に横スクロールモードを解除する
        GameManager.Instance.OnPlayerRestartObservable.Subscribe(_ => isOnlyMoveToSide = false);

        //会話が始まったらプレイヤーを動かなくする
        DialogueManager.Instance.OnDialogueStartObservable.Subscribe(_ => GoToImmovable()).AddTo(gameObject);

        //会話が終わったらプレイヤーを動けるようにする
        DialogueManager.Instance.OnDialogueEndObservable.Delay(TimeSpan.FromSeconds(0.1f)).Subscribe(_ => GoToMovable()).AddTo(gameObject);

        groundChecker.LayerMaskProperty.Subscribe(layerMask => SetFootSe(layerMask)).AddTo(gameObject);

    }

    /// <summary>
    /// 横スクロールと通常移動とを切り替える関数
    /// </summary>
    /// <param name="setModeBool"></param>
    public void SetIfOnlyMoveToSide(bool setModeBool)
    {
        isOnlyMoveToSide = setModeBool;
    }
    /// <summary>
    /// Moveの更新関数
    /// </summary>

    // Update is called once per frame
    public void UpdateMove()
    {
        gravityNormalProperty.Value = gravity.NormalVec;
        cameraRotProperty.Value = pov.m_HorizontalAxis.Value;
        //接地状態の時
        if (groundChecker.IsGround)
        {
            //ジャンプボタンが押されたときゲームがポーズしていなくてプレイヤーが動ける状態ならジャンプ
            if (inputManager.JumpInput.Value && !GameManager.Instance.IsPausing && isMovable)
            {
                Jump();
            }
        }
       
        groundChecker.UpdateGroundCheck();

        //動ける状態でmoveDirectionの大きさが0ではない場合プレイヤーを前方向に移動させる
        if (isMovable)
        {
            if (moveDirection.magnitude > 0)
            {
                rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.deltaTime); //入力により向いた方向に進む
            }
        }

        else
        {
            moveDirection = Vector3.zero;
        }


        if (isGround)
        {
            animator.SetFloat("Speed", moveDirection.magnitude);
        }

        else
        {
            animator.SetFloat("Speed", 0f);

        }
        isGround = groundChecker.IsGround;
        animator.SetBool("isGrounded", isGround);

    }

    /// <summary>
    /// スターリングで移動後着地した時にスターリングに乗る前の入力ベクトルを受け取ったまま勝手に歩き始めるバグを防ぐため、着地した際に入力ベクトルをリセットする
    /// </summary>
    public void InitializeVector()
    {
        DebugLog.Log("入力を初期化");
        GetAdjustedVector(Vector3.zero);
    }

    /// <summary>
    /// Playerを進行方向に向ける
    /// </summary>
    /// <param name="input"></param>
    void GetAdjustedVector(Vector3 input)
    {
        DebugLog.Log(input.ToString());
        DecideAxis();
        //地面の垂線とbasedVectorとの外積を移動軸の右方向とする
        right = Vector3.Cross(gravity.NormalVec, basedVector).normalized;
        //右方向と地面の垂線との外積を移動軸の前方向とする
        forward = Vector3.Cross(right, gravity.NormalVec).normalized;

        //プレイヤーにかかる重力が上方向で、惑星上にいない場合、右方向を逆にする
        right = isUpSideDown && !isUp ? -right : right;
        moveDirection = right * input.x + forward * input.y;
        moveDirection = moveDirection.normalized;

        if (moveDirection.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection, transform.up); //自身の上方向を軸としてmoveDirectionのほうにプレイヤーを向ける
        }

       

    }

    /// <summary>
    /// gravity.NormalVecに対してどのベクトルとの外積を進行ベクトルにおける右方向と定義するかを決める
    /// 基本的にカメラの向いている方向。カメラが真上にある→カメラの上方向、プレイヤーが上下さかさまになっている→カメラの後ろ方向
    /// </summary>
    /// <returns></returns>
    void DecideAxis()
    {

        planetIsSmallBox = gravity.PlanetObj != null && gravity.Planet.PlanetTypeParam == Planet.PlanetType.SmallBox;
        //地面の垂線とカメラの向いている方向とがほぼ平行(カメラがプレイヤーの真上にある)の場合は
        //gravity.NormalVecとカメラの上方向との外積を進行ベクトルの右方向として定義する
        if (Vector3.Dot(gravity.NormalVec, mainCamera.transform.forward) < -parallelThreshould)
        {
            isUp = true;
            isUpSideDown = false;
            basedVector = mainCamera.transform.up;

        }
        //地面の垂線とカメラの上方向がほぼ正反対の方向で惑星上にプレイヤーがいるとき(プレイヤーが上下さかさまに映っているとき)
        //gravity.NormalVecとカメラの後ろ方向との外積を進行ベクトルの右方向として定義する
        else if (Vector3.Dot(gravity.NormalVec, mainCamera.transform.up) < -upsideDownThreshould && planetIsSmallBox)
        {
            isUp = false;
            isUpSideDown = false;
            basedVector = -mainCamera.transform.forward;
        }
        //上記以外の場合はgravity.NormalVecとカメラの前方向との外積を進行ベクトルの右方向として定義する
        else
        {
            isUp = false;
            isUpSideDown = Vector3.Dot(gravity.NormalVec, mainCamera.transform.up) < -upsideDownThreshould; 
            basedVector = mainCamera.transform.forward;

        }
    }

    /// <summary>
    /// 接地している場合にジャンプ入力をすることでジャンプさせる
    /// </summary>

    public void Jump()
    {

        audioSource.PlayOneShot(jumpAudioclip);
        groundChecker.DisableToCheckGround();
        rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
       
    }

    public void AttackJump()
    {
        DebugLog.Log("攻撃ジャンプ");
        rb.AddForce(transform.up * attackJumpPower, ForceMode.Impulse);
        audioSource.PlayOneShot(stepOnAudioClip);
    }

    /// <summary>
    /// 足音を鳴らす関数。→アニメーションクリップのイベントとして登録
    /// </summary>
    void PlayFootSound()
    {
        audioSource.volume = 1;
        audioSource.PlayOneShot(footAudioClip);

    }

    private void SetFootSe(string layerName)
    {
        // ヒットしたレイヤー名からレイヤー番号を取得
        int layerIndex = LayerMask.NameToLayer(layerName);

        // 見つかった要素を格納する変数
        FootSeSet foundSeSet = default;
        bool found = false;

        // footSeSetsの各要素をループして比較
        foreach (var footSeSet in footSeSets)
        {
            // ビット演算を使用して、レイヤーマスクにヒットしたレイヤーが含まれているかを確認
            if (((1 << layerIndex) & footSeSet.groundLayer) != 0)
            {
                foundSeSet = footSeSet;
                found = true;
                break; // 一致するものが見つかったらループを抜ける
            }
        }

        if (found)
        {
            footAudioClip = foundSeSet.footSe;
        }
        else
        {
            Debug.LogWarning($"足音のオーディオクリップが、レイヤー名 '{layerName}' に一致する要素で見つかりませんでした。");
            // デフォルトの足音を鳴らすなどのフォールバック処理
        }
    }

    public override void GoToMovable()
    {
        DebugLog.Log("動けるように");
        isMovable = true;
        InitializeVector();
    }

    public override void GoToImmovable()
    {
        isMovable = false;
        animator.SetFloat("Speed", 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, moveDirection * 5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, forward * 5f);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, right * 5f);

        //Gizmos.color = Color.green;
        //Gizmos.DrawRay(transform.position, moveDirection * 5f);


    }

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

}