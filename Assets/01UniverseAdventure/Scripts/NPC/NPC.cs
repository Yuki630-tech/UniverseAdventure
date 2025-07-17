using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Gravity))]
public class NPC : MonoBehaviour
{
    [Tooltip("物理挙動"), SerializeField] private Rigidbody rb;
    [Tooltip("惑星の法線を習得するための重力コンポーネント"), SerializeField] private Gravity gravity;
    private Vector3 forward;

    [Header("目的地のトランスフォーム"), SerializeField] private Transform destinationTrans;
    [Header("目的地まで行く移動速度"), SerializeField] private float moveSpeed;

    public enum NPCState
    {
        Idle,
        Move
    }

    private NPCState currentState;

    private void Awake()
    {
        currentState = NPCState.Idle;
    }

    private void Update()
    {
        switch (currentState)
        {
            case NPCState.Idle:

                break;
            case NPCState.Move:
                UpdateMove();

                if(Vector3.Distance(transform.position, destinationTrans.position) <= 0.01f)
                {
                    transform.position = destinationTrans.position;
                    ChangeToIdleState();
                }

                break;

        }
    }

    private void ChangeToIdleState()
    {
        currentState = NPCState.Idle;
    }

    public void ChangeToMoveState()
    {
        currentState = NPCState.Move;
    }

    private void UpdateMove()
    {
        Vector3 direction = (destinationTrans.position - transform.position).normalized;
        Vector3 right = Vector3.Cross(gravity.NormalVec, direction).normalized;
        Vector3 forward = Vector3.Cross(right, gravity.NormalVec).normalized;
        rb.MovePosition(rb.position + forward * moveSpeed * Time.deltaTime);
    }

    public void SetDestinationAndSpeed(Transform setDestinationTrans, float setSpeed)
    {
        destinationTrans = setDestinationTrans;
        moveSpeed = setSpeed;
    }
}
