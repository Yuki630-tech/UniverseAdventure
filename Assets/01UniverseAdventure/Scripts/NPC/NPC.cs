using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Gravity))]
public class NPC : MonoBehaviour
{
    [Tooltip("��������"), SerializeField] private Rigidbody rb;
    [Tooltip("�f���̖@�����K�����邽�߂̏d�̓R���|�[�l���g"), SerializeField] private Gravity gravity;
    private Vector3 forward;

    [Header("�ړI�n�̃g�����X�t�H�[��"), SerializeField] private Transform destinationTrans;
    [Header("�ړI�n�܂ōs���ړ����x"), SerializeField] private float moveSpeed;

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
