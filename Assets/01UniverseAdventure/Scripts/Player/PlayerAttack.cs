using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Tooltip("�������Z"), SerializeField] Rigidbody rb;
    [Tooltip("�f���̖@������ɃW�����v���邽�߂�Gravity�R���|�[�l���g���擾"), SerializeField] Gravity gravity;
    [Tooltip("�W�����v��"), SerializeField] float jumpPower = 2f;
    public void JumpWhenAttack()
    {
        rb.AddForce(gravity.NormalVec * jumpPower, ForceMode.Impulse);
    }
}
