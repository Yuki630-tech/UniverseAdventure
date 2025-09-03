using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("€–S‚ÉŠÖ‚·‚éİ’è")]
    [Tooltip("€–S‚µ‚½Û‚ÌyƒXƒP[ƒ‹"), SerializeField] private float scaleOfDieY = 20;
    [Tooltip("€–S‚µ‚Ä‚©‚çÁ‚¦‚é‚Ü‚Å‚ÌŠÔ"), SerializeField] private float destroyInterval = 1f;
    [Tooltip("€–S‚É”ñƒAƒNƒeƒBƒu‚É‚·‚éAttackDetecter"), SerializeField] private AttackDetecter attackDetecter;
    protected StateMachine<EnemyBaseState, EnemyBase> StateMachineBase = new StateMachine<EnemyBaseState, EnemyBase>();
    public Subject<Unit> OnDieByStepOnSubject { get; private set; } = new Subject<Unit>();
    public Subject<Unit> OnDestroyByStepOnSubject { get; private set; } = new Subject<Unit>();

    public IObservable<Unit> OnDieObservable => OnDieByStepOnSubject;
    public IObservable<Unit> OnDestroyByStepOnObservable => OnDestroyByStepOnSubject;

    public float DestroyInterval { get => destroyInterval; }
    public float ScaleOfDieY { get => scaleOfDieY; }



    public enum EnemyBaseState
    {
        DieByStepOn,
        DieBySpin,
        None
    }

    protected virtual void Awake()
    {
        Initialize();
        OnDieObservable.Subscribe(_ =>
        {
            attackDetecter?.gameObject.SetActive(false);
            
        });

        GameManager.Instance.OnPlayerRestartObservable.Subscribe(_ =>
        {
            GetComponent<Collider>().enabled = true;
            attackDetecter?.gameObject.SetActive(true);
            StateMachineBase?.ChangeState(EnemyBaseState.None, this);
        });
    }

    protected virtual void Update()
    {
        if(StateMachineBase.CurrentStateName != EnemyBaseState.None)
        {
            StateMachineBase.Update(this, Time.deltaTime);
        }
    }

    protected virtual void Initialize()
    {
        StateMachineBase.AddState(EnemyBaseState.DieByStepOn, new EnemyDieByStepOn());
        StateMachineBase.AddState(EnemyBaseState.None, null);
        StateMachineBase.ChangeState(EnemyBaseState.None, this);
    }

    /// <summary>
    /// “¥‚İ‚Â‚¯‚ÌAttackDetecter‚ÉG‚ê‚½Û‚ÌƒCƒxƒ“ƒg‚Æ‚µ‚Ä“o˜^‚·‚é
    /// </summary>
    public virtual void DieByStepOn()
    {
        Die();
        if (StateMachineBase.CurrentStateName == EnemyBaseState.DieByStepOn || StateMachineBase.CurrentStateName == EnemyBaseState.DieBySpin) return;
        StateMachineBase.ChangeState(EnemyBaseState.DieByStepOn, this);
    }

    /// <summary>
    /// ƒXƒsƒ“UŒ‚‚É‚æ‚éAttackDetecter‚ÉG‚ê‚½Û‚ÌƒCƒxƒ“ƒg‚Æ‚µ‚Ä“o˜^‚·‚é
    /// </summary>
    public virtual void DieBySpin()
    {
        Die();
        if (StateMachineBase.CurrentStateName == EnemyBaseState.DieByStepOn || StateMachineBase.CurrentStateName == EnemyBaseState.DieBySpin) return;
        StateMachineBase.ChangeState(EnemyBaseState.DieBySpin, this);
    }
    public void DestroyEnemy()
    {
        gameObject.SetActive(false);
    }

    protected abstract void Die();
}
