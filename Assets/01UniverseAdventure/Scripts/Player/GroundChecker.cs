using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [Tooltip("レイの始点"), SerializeField] float center;
    [Tooltip("レイの先端の球の半径"), SerializeField] float radius;
    [Tooltip("レイの長さ"), SerializeField] float length;
    [Tooltip("自分自身が持っているGravityコンポーネント"), SerializeField] Gravity gravity;


    [Header("接地しているかどうか"), SerializeField] bool isGround;

    [Header("接地確認を行っているかどうか"), SerializeField] bool isGroundCheckEffective = true;


    RaycastHit hit;

    public bool IsGround { get => isGround; }

    private void Start()
    {
        
    }

    public void UpdateGroundCheck()
    {
        //ジャンプした時しばらくグランドチェックしないようにする(ジャンプした瞬間は地面についていて接地判定になってしまうため)
        if (isGroundCheckEffective)
        {
            //レイの始点をプレイヤーの座標(足元)からgravityのNormalVecの方向にcenter分だけ移動した場所に設定する→プレイヤーが惑星に沿って歩いた際にも
            //レイが適切に飛ぶように。始点からNormalVecとは逆向き(プレイヤーの足元に向かって)レイを飛ばし接地判定をする。
            var origin = transform.position + gravity.NormalVec * center;
            if (Physics.SphereCast(origin, radius, -gravity.NormalVec, out hit, length))
            {
                
                if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Planet"))
                {
                    isGround = true;
                }
               
            }

            else
            {
                isGround = false;
            }
        }

        else
        {
            isGround = false;
        }
        
    }
    /// <summary>
    /// ジャンプ直後に接地判定を取らないようにしばらく接地判定を行わないようにする関数。
    /// </summary>
    public async void DisableToCheckGround()
    {
        isGroundCheckEffective = false;
        await UniTask.Delay(1000);
        isGroundCheckEffective = true;
    }

    private void OnDrawGizmos()
    {
        var origin = transform.position + center * gravity.NormalVec;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(origin, -gravity.NormalVec * length);

        Gizmos.DrawWireSphere(origin - transform.up * length, radius);
    }
}
