using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Gravity))]
public class NPCInTimeline : MonoBehaviour
{
    [Tooltip("物理挙動"), SerializeField] private Rigidbody rb;
    [Tooltip("惑星の法線を習得するための重力コンポーネント"), SerializeField] private Gravity gravity;
    [Tooltip("アニメーター"), SerializeField] private Animator animator;
    private Vector3 forward;

    [Header("目的地のトランスフォーム"), SerializeField] private Transform destinationTrans;
    [Header("目的地まで行く移動速度"), SerializeField] private float moveSpeed;
    [Header("目的地の方向に向く回転速度"), SerializeField] private float rotateSpeed;

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

                break;

        }
    }

    private void ChangeToIdleState()
    {
        currentState = NPCState.Idle;
        animator.SetBool(AnimationParametaName.IsMove, false);
        rb.velocity = Vector3.zero;
    }

    public async void ChangeToMoveState()
    {
        Vector3 direction = (destinationTrans.position - transform.position).normalized;
        Vector3 right = Vector3.Cross(gravity.NormalVec, direction).normalized;
        Vector3 forward = Vector3.Cross(right, gravity.NormalVec).normalized;
        var look = Quaternion.LookRotation(forward);
        await LookAtDestination(look);
        currentState = NPCState.Move;
        animator.SetBool(AnimationParametaName.IsMove, true);
    }

    private async UniTask LookAtDestination(Quaternion setLook)
    {
        while(transform.rotation != setLook)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, setLook, rotateSpeed * Time.deltaTime);
            await UniTask.Yield();
        }
    }

    private void UpdateMove()
    {
        Vector3 direction = (destinationTrans.position - transform.position).normalized;
        Vector3 right = Vector3.Cross(gravity.NormalVec, direction).normalized;
        Vector3 forward = Vector3.Cross(right, gravity.NormalVec).normalized;
        var lookRot = Quaternion.LookRotation(forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);
        rb.MovePosition(rb.position + forward * moveSpeed * Time.deltaTime);
    }

    public void SetDestinationAndSpeed(Transform setDestinationTrans, float setSpeed)
    {
        destinationTrans = setDestinationTrans;
        moveSpeed = setSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destination"))
        {
            ChangeToIdleState();
        }
    }

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }
}
