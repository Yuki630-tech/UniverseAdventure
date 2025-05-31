using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class PlayerHipDrop : MonoBehaviour
{
    [Tooltip("�q�b�v�h���b�v���͂����m���邽�߂�InputManager"), SerializeField] InputManager inputManager;
    [Tooltip("���g��Groundchecker"), SerializeField] GroundChecker groundChecker;
    [SerializeField] Rigidbody rb;
    [SerializeField] Gravity gravity;
    [SerializeField] float dropPower;
    [Tooltip("�q�b�v�h���b�v�̓����蔻��"), SerializeField] GameObject dropHit;

    [Header("�q�b�v�h���b�v�\���ǂ���"), SerializeField] bool canHipDrop;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        dropHit.SetActive(false);
    }

    private void Update()
    {
        var gravityDirection = -transform.up;

        //�v���C���[�̉������Ƀ��C���΂������������̂�Gravity�ɓo�^����Ă���d�͌��̘f�����A�������͏d�͌����Ȃ��ꍇ�̓q�b�v�h���b�v�\�Ƃ���
        //���q�b�v�h���b�v�̐����ŏd�͂̐؂�ւ�肪�ǂ��t���Ȃ��Ȃ��ĕ��V�������鋓����h���B(���n�ł���n�ʂ��^���ɂ���Ƃ��̂݃q�b�v�h���b�v����悤��)
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
    /// �q�b�v�h���b�v
    /// </summary>
    void HipDrop()
    {
        var gravityDirection = -transform.up;
        rb.AddForce(gravityDirection * dropPower, ForceMode.Impulse);
        ActivateDropCpl().Forget();
    }

    /// <summary>
    /// �q�b�v�h���b�v���̓����蔻���L���ɂ���֐�
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
