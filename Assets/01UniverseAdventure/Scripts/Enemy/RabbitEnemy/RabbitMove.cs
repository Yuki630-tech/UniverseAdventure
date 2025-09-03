using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitMove : IState<RabbitEnemy>
{
    Vector3 directionBase;
    Vector3 destination;
    Vector3 start;
    float currentTime;
    public void Enter(RabbitEnemy owner)
    {
        start = owner.transform.position;
        owner.Animator.SetBool("IsRunning", true);
        currentTime = 0f;
        MakeDestination(owner);
    }

    public void Update(RabbitEnemy owner, float deltaTime)
    {
        currentTime += deltaTime;
        Move(owner, deltaTime);
        Debug.DrawRay(start, directionBase);

        if (currentTime >= owner.MoveInterval)
        {
            owner.StateMachine.ChangeState(RabbitEnemy.RabbitState.Idle, owner);
        }
    }

    public void Exit(RabbitEnemy owner)
    {
        //‚Æ‚­‚É‚È‚µ
    }

    private void Move(RabbitEnemy owner, float deltaTime)
    {
        Vector3 direction = owner.Gravity.PlanetObj != null && owner.Gravity.Planet.PlanetTypeParam == Planet.PlanetType.Sphere ? owner.DirectionMakerOnSphere.MakeVector(destination): (destination - owner.transform.position).normalized;
       
        owner.transform.rotation = Quaternion.LookRotation(direction, owner.transform.up);
        owner.Rb.MovePosition(owner.Rb.position + direction * owner.MoveSpeed * deltaTime);
        if(Vector3.Distance(owner.transform.position, destination) <= 0.1f)
        {
            owner.StateMachine.ChangeState(RabbitEnemy.RabbitState.Idle, owner);
        }
    }

    private void MakeDestination(RabbitEnemy owner)
    {
        int count = 0;
        bool foundGround = false;
        while (!foundGround)
        {
            directionBase = owner.transform.right * Random.Range(owner.MoveRangeX.x, owner.MoveRangeX.y) + owner.transform.forward * Random.Range(owner.MoveRangeZ.x, owner.MoveRangeZ.y);
            Vector3 destinationBase = owner.transform.position + directionBase;
            foundGround = Physics.Raycast(destinationBase, -owner.transform.up, owner.Distance);
            if (count >= 10)
            {
                owner.StateMachine.ChangeState(RabbitEnemy.RabbitState.Idle ,owner);
                return; 
            }
            count++;
        }
        destination = owner.transform.position + directionBase;
    }
}
