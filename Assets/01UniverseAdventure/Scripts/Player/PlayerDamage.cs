using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UniRx;
using System;
using Cysharp.Threading.Tasks;

public class PlayerDamage : MonoBehaviour
{
    [Tooltip("�v���C���[�̃X�e�[�^�X�𑀍삷��R���|�[�l���g"), SerializeField] PlayerStatus playerStatus;
    [Tooltip("�_���[�W���󂯂��Ƃ��̉���炷�I�[�f�B�I�\�[�X"), SerializeField] AudioSource playerAudioSource;
    [Tooltip("�_���[�W���󂯂��Ƃ��̉�"), SerializeField] AudioClip damageAudioClip;
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider col;
    [Tooltip("���G����"), SerializeField] float invincibleTime = 0.5f;
    [Tooltip("�_�ł̊Ԋu"), SerializeField] float flashInterval = 0.5f;
    [Tooltip("�m�b�N�o�b�N�����"), SerializeField] float knockBackPower;
    [SerializeField] Animator animator;
    [Header("���G��Ԃ̊Ԃɓ_�ł����郌���_���[�̃��X�g"), SerializeField] Renderer[] renderers;
    [Header("���G��Ԃ��ǂ���"), SerializeField] bool isInvincible;
    [Header("���G��Ԃ̐؂�ւ����s�����ǂ���"), SerializeField] bool isEnableToSwitchInvicibleMode;
    
    //�R���[�`�����~�߂邱�Ƃ��ł���悤�ɕێ����邽�߂̕ϐ�
    Coroutine flashCoroutine;
    /// <summary>
    /// ���̒l��true�̊Ԃ̓v���C���[�̓_���[�W�X�e�[�g�ɂ���
    /// </summary>
    public bool IsDamaged {  get; private set; }
    public bool IsInvincible { get => isInvincible; }
    public PlayerStatus PlayerStatus { get => playerStatus; }

    private void Awake()
    {
        isEnableToSwitchInvicibleMode = true;
        renderers = GetComponentsInChildren<Renderer>();
        //�Q�[�����|�[�Y���Ă���ԃ_���[�W���󂯂Ȃ��悤�ɂ���
        GameManager.Instance?.OnPauseGameObservable.Subscribe(_ => ChangeInvicibleValue(true)).AddTo(this);

        //�Q�[�����ăX�^�[�g�������Ƀ_���[�W���󂯂�悤�ɂ���
        GameManager.Instance?.OnUnPauseGameObservable.Subscribe(_ => ChangeInvicibleValue(false)).AddTo(this);

    }

    /// <summary>
    /// �_���[�W���󂯂���hp�����炵�Ȃ���m�b�N�o�b�N���鏈���B
    /// </summary>
    /// <param name="data"></param>
    public void OnTakeDamage(AttackData data)
    {
        
        animator.SetTrigger("Damage");
        
        //�_���[�W��Ԃɂ��遨false�ɂȂ�����=�_���[�W�A�j���[�V�������I�������Move�X�e�[�g�ɐ؂�ւ��B
        IsDamaged = true;
        playerStatus.TakeDamage(data.Damage);
        //�������Ƀv���C���[���m�b�N�o�b�N������
        var knockBack = data.KnockBack;
        var look = Quaternion.LookRotation(transform.forward);
        knockBack = look * knockBack;
        //�v���C���[���m�b�N�o�b�N��������Ƃ͋t�̕����Ɍ����悤�ɂ��遨���ɔ�΂���鋓����
        Vector3 lookVec = new Vector3(knockBack.x, 0f, knockBack.z);
        if(lookVec != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(-lookVec);

        }
        rb.velocity = knockBack * knockBackPower;

        //���ׂ�₷���悤�ɂ���
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
    /// �_���[�W��Ԃ���񕜂���֐����_���[�W�A�j���[�V�����̃A�j���[�V�����X�e�[�g���甲���o�����Ƃ��ɌĂяo���A�ړ��ł����Ԃɖ߂�
    /// </summary>
    public void OnRecovary()
    {
        IsDamaged = false;
        rb.velocity = Vector3.zero;

        col.material.staticFriction = 0f;
        col.material.dynamicFriction = 0f;
    }

    /// <summary>
    /// �_���[�W���󂯂��サ�΂炭���G��ԂɂȂ�悤�ɂ���R���[�`���B
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
    /// �N�G�X�g���I�������̃C�x���g���A�Q�[�����|�[�Y�������ȂǂɃ_���[�W���󂯂邱�Ƃ̂Ȃ��悤�ɓ���ȃ^�C�~���O�Ŗ��G��Ԃ�؂�ւ���֐��B
    /// </summary>
    /// <param name="setInvincible"></param>

    public void ChangeInvicibleValue(bool setInvincible)
    {
        isInvincible = setInvincible;
        isEnableToSwitchInvicibleMode = !setInvincible;
    }

    /// <summary>
    /// ���G���ɓ_�ł�����R���[�`���B
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
    /// ���G��Ԃ��I��������ɓ_�ł��~�߂�֐��B
    /// </summary>
    public void StopFlash()
    {
        if(flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);

        }
        //�_�ł��I��������Ƀ����_���[�������ɂɂȂ��ăv���C���[�̎p�������Ȃ��Ȃ邱�Ƃ�h�����߂Ƀ����_���[��L���ɂ��Ă����B
        foreach (var renderer in renderers)
        {
            renderer.enabled = true;
        }
    }


}
