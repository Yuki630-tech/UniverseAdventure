using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Tooltip("物理演算"), SerializeField] Rigidbody rb;
    [Tooltip("惑星の法専方向にジャンプするためにGravityコンポーネントを取得"), SerializeField] Gravity gravity;
    [Tooltip("ジャンプ力"), SerializeField] float jumpPower = 2f;
    public void JumpWhenAttack()
    {
        rb.AddForce(gravity.NormalVec * jumpPower, ForceMode.Impulse);
    }
}
