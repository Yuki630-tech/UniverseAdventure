using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UniRx;
using System;
using Cysharp.Threading.Tasks;

public class PlayerDamage : MonoBehaviour
{
    [Tooltip("プレイヤーのステータスを操作するコンポーネント"), SerializeField] PlayerStatus playerStatus;
    [Tooltip("ダメージを受けたときの音を鳴らすオーディオソース"), SerializeField] AudioSource playerAudioSource;
    [Tooltip("ダメージを受けたときの音"), SerializeField] AudioClip damageAudioClip;
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider col;
    [Tooltip("無敵時間"), SerializeField] float invincibleTime = 0.5f;
    [Tooltip("点滅の間隔"), SerializeField] float flashInterval = 0.5f;
    [Tooltip("ノックバックする力"), SerializeField] float knockBackPower;
    [SerializeField] Animator animator;
    [Header("無敵状態の間に点滅させるレンダラーのリスト"), SerializeField] Renderer[] renderers;
    [Header("無敵状態かどうか"), SerializeField] bool isInvincible;
    [Header("無敵状態の切り替えを行うかどうか"), SerializeField] bool isEnableToSwitchInvicibleMode;
    
    //コルーチンを止めることができるように保持するための変数
    Coroutine flashCoroutine;
    /// <summary>
    /// この値がtrueの間はプレイヤーはダメージステートにする
    /// </summary>
    public bool IsDamaged {  get; private set; }
    public bool IsInvincible { get => isInvincible; }
    public PlayerStatus PlayerStatus { get => playerStatus; }

    private void Awake()
    {
        isEnableToSwitchInvicibleMode = true;
        renderers = GetComponentsInChildren<Renderer>();
        //ゲームをポーズしている間ダメージを受けないようにする
        GameManager.Instance?.OnPauseGameObservable.Subscribe(_ => ChangeInvicibleValue(true)).AddTo(this);

        //ゲームが再スタートした時にダメージを受けるようにする
        GameManager.Instance?.OnUnPauseGameObservable.Subscribe(_ => ChangeInvicibleValue(false)).AddTo(this);

    }

    /// <summary>
    /// ダメージを受けた時hpを減らしながらノックバックする処理。
    /// </summary>
    /// <param name="data"></param>
    public void OnTakeDamage(AttackData data)
    {
        
        animator.SetTrigger("Damage");
        
        //ダメージ状態にする→falseになった時=ダメージアニメーションが終わった時Moveステートに切り替わる。
        IsDamaged = true;
        playerStatus.TakeDamage(data.Damage);
        //後ろ方向にプレイヤーをノックバックさせる
        var knockBack = data.KnockBack;
        var look = Quaternion.LookRotation(transform.forward);
        knockBack = look * knockBack;
        //プレイヤーをノックバックする方向とは逆の方向に向くようにする→後ろに飛ばされる挙動に
        Vector3 lookVec = new Vector3(knockBack.x, 0f, knockBack.z);
        if(lookVec != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(-lookVec);

        }
        rb.velocity = knockBack * knockBackPower;

        //すべりやすいようにする
        col.material.staticFriction = 0.5f;
        col.material.dynamicFriction = 0.5f;
        if(playerStatus.Hp.Value > 0)
        {
            StartCoroutine(GoToInvincibleMode());
            flashCoroutine = StartCoroutine(Flash());
        }
        playerAudioSource.PlayOneShot(damageAudioClip);

    }

    /// <summary>
    /// ダメージ状態から回復する関数→ダメージアニメーションのアニメーションステートから抜け出したときに呼び出し、移動できる状態に戻す
    /// </summary>
    public void OnRecovary()
    {
        IsDamaged = false;
        rb.velocity = Vector3.zero;

        col.material.staticFriction = 0f;
        col.material.dynamicFriction = 0f;
    }

    /// <summary>
    /// ダメージを受けた後しばらく無敵状態になるようにするコルーチン。
    /// </summary>
    /// <returns></returns>

    IEnumerator GoToInvincibleMode()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        if (!isEnableToSwitchInvicibleMode)
        {
            yield return new WaitUntil(() => isEnableToSwitchInvicibleMode);
        }
        isInvincible = false;
    }

    /// <summary>
    /// クエストが終わった後のイベント時、ゲームをポーズした時などにダメージを受けることのないように特殊なタイミングで無敵状態を切り替える関数。
    /// </summary>
    /// <param name="setInvincible"></param>

    public void ChangeInvicibleValue(bool setInvincible)
    {
        isInvincible = setInvincible;
        isEnableToSwitchInvicibleMode = !setInvincible;
    }

    /// <summary>
    /// 無敵中に点滅させるコルーチン。
    /// </summary>
    /// <returns></returns>
    IEnumerator Flash()
    {
        while (isInvincible)
        {
            foreach(var renderer in renderers)
            {
                renderer.enabled = !renderer.enabled;
            }
           
            yield return new WaitForSeconds(flashInterval);
        }

        foreach(var renderer in renderers)
        {
            renderer.enabled = true;
        }
    }

    /// <summary>
    /// 無敵状態が終わった時に点滅を止める関数。
    /// </summary>
    public void StopFlash()
    {
        if(flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);

        }
        //点滅が終わった時にレンダラーが無効にになってプレイヤーの姿が見えなくなることを防ぐためにレンダラーを有効にしておく。
        foreach (var renderer in renderers)
        {
            renderer.enabled = true;
        }
    }


}
