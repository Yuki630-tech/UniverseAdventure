using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class PlayerHipDrop : MonoBehaviour
{
    [Tooltip("ヒップドロップ入力を感知するためのInputManager"), SerializeField] InputManager inputManager;
    [Tooltip("自身のGroundchecker"), SerializeField] GroundChecker groundChecker;
    [SerializeField] Rigidbody rb;
    [SerializeField] Gravity gravity;
    [SerializeField] float dropPower;
    [Tooltip("ヒップドロップの当たり判定"), SerializeField] GameObject dropHit;

    [Header("ヒップドロップ可能かどうか"), SerializeField] bool canHipDrop;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        dropHit.SetActive(false);
    }

    private void Update()
    {
        var gravityDirection = -transform.up;

        //プレイヤーの下方向にレイを飛ばしあたったものがGravityに登録されている重力源の惑星か、もしくは重力源がない場合はヒップドロップ可能とする
        //→ヒップドロップの勢いで重力の切り替わりが追い付かなくなって浮遊し続ける挙動を防ぐ。(着地できる地面が真下にあるときのみヒップドロップするように)
        if (Physics.Raycast(transform.position, gravityDirection, out hit, Mathf.Infinity))
        {
            canHipDrop = hit.collider.gameObject == gravity.PlanetObj || gravity.PlanetObj == null;

        }
        if (inputManager.IsHipDrop.Value && canHipDrop)
        {
            HipDrop();
        }
    }

    /// <summary>
    /// ヒップドロップ
    /// </summary>
    void HipDrop()
    {
        var gravityDirection = -transform.up;
        rb.AddForce(gravityDirection * dropPower, ForceMode.Impulse);
        ActivateDropCpl().Forget();
    }

    /// <summary>
    /// ヒップドロップ時の当たり判定を有効にする関数
    /// </summary>
    /// <returns></returns>
    async UniTaskVoid ActivateDropCpl()
    {
        dropHit.SetActive(true);
        await UniTask.WaitUntil(() => groundChecker.IsGround);
        rb.velocity = Vector3.zero;
        await UniTask.Delay(500);
        dropHit.SetActive(false);
    }


}
