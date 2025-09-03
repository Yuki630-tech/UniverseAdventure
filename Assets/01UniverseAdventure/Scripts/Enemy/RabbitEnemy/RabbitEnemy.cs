using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class RabbitEnemy : EnemyBase
{
    [Header("すべてのステートで使用する設定")]
    [Tooltip("物理演算"), SerializeField] private Rigidbody rb;
    [Tooltip("Animator"), SerializeField] private Animator animator;

    [Header("IdleStateに関する設定")]
    [Tooltip("待機する時間"), SerializeField] private float idleInterval = 2f;

    [Header("MoveStateに関する設定")]
    [Tooltip("移動する時間"), SerializeField] private float moveInterval = 2f;
    [Tooltip("移動速度"), SerializeField] private float moveSpeed = 5f;
    [Tooltip("惑星情報取得のためのGravityコンポーネント"), SerializeField] private Gravity gravity;
    [Tooltip("移動する方向を決める機能"), SerializeField] private DirectionMakerOnSphere directionMakerOnSphere;
    [Tooltip("徘徊する範囲のx軸方向最小値(x), 最大値(y)"), SerializeField] private Vector2 moveRangeX;
    [Tooltip("徘徊する範囲のy軸方向最小値(x), 最大値(y)"), SerializeField] private Vector2 moveRangeZ;
    [Tooltip("行先に地面がある確認するレイの長さ"), SerializeField] private float distance;

    [Header("Debug用。現在のステート名")]
    [SerializeField] private RabbitState stateName;

    public StateMachine<RabbitState, RabbitEnemy> StateMachine { get; private set; } = new StateMachine<RabbitState, RabbitEnemy>();
    public Rigidbody Rb { get => rb; }
    public Animator Animator { get => animator; }
    public float IdleInterval { get => idleInterval; }
    public float MoveInterval { get => moveInterval; }
    public Gravity Gravity { get => gravity; }
    /// <summary>
    /// 徘徊する範囲のx軸方向最小値→x, 最大値→y
    /// </summary>
    public Vector2 MoveRangeX { get => moveRangeX; }

    /// <summary>
    /// 徘徊する範囲のz軸方向最小値→x, 最大値→y
    /// </summary>
    public Vector2 MoveRangeZ { get => moveRangeZ; }
    public DirectionMakerOnSphere DirectionMakerOnSphere { get => directionMakerOnSphere; }
    public float MoveSpeed { get => moveSpeed; }
    public float Distance { get => distance; }

    public enum RabbitState
    {
        Idle,
        Move,
        Chase,
        DieByStepOn,
        DieBySpin,
        None
    }

    protected override void Awake()
    {
        base.Awake();
        directionMakerOnSphere = new DirectionMakerOnSphere(transform, gravity);
        Initialize();
        GameManager.Instance.OnPlayerRestartObservable.Subscribe(_ => StateMachine.ChangeState(RabbitState.Idle, this));
      
    }

    protected override void Initialize()
    {
        base.Initialize();
        StateMachine.AddState(RabbitState.Idle, new RabbitIdle());
        StateMachine.AddState(RabbitState.Move, new RabbitMove());
        StateMachine.AddState(RabbitState.None, null);
        StateMachine.ChangeState(RabbitState.Idle, this);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(StateMachine.CurrentStateName != RabbitState.None)
        {
            StateMachine.Update(this, Time.deltaTime);

        }

        stateName = StateMachine.CurrentStateName;
    }

    private void OnDrawGizmos()
    {
        Vector3 point1 = transform.position + transform.right * MoveRangeX.x + transform.forward * MoveRangeZ.x;
        Vector3 point2 = transform.position + transform.right * MoveRangeX.x + transform.forward * MoveRangeZ.y;
        Vector3 point3 = transform.position + transform.right * MoveRangeX.y + transform.forward * MoveRangeZ.x;
        Vector3 point4 = transform.position + transform.right * MoveRangeX.y + transform.forward * MoveRangeZ.y;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(point1, 0.5f);
        Gizmos.DrawSphere(point2, 0.5f);
        Gizmos.DrawSphere(point3, 0.5f);
        Gizmos.DrawSphere(point4, 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.up * distance);
    }

    protected override void Die()
    {
        StateMachine.ChangeState(RabbitState.None, this);
    }
}
