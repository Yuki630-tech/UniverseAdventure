using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitIdle : IState<RabbitEnemy>
{
    private float currentTime;
    public void Enter(RabbitEnemy owner)
    {
        owner.Animator.SetBool("IsRunning", false);
        currentTime = 0f;
        owner.Rb.velocity = Vector3.zero;
    }

    public void Update(RabbitEnemy owner, float deltaTime)
    {
        currentTime += deltaTime;
        //DebugLog.Log($"Idle:{currentTime}");
        if(currentTime >= owner.IdleInterval)
        {
            owner.StateMachine.ChangeState(RabbitEnemy.RabbitState.Move, owner);
        }
    }

    public void Exit(RabbitEnemy owner)
    {
        
    }
}
