using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SetMoveModeObject : MonoBehaviour
{
    [Tooltip("�v���C���[���G�ꂽ�Ƃ��Ƀv���C���[�����X�N���[�����[�h�ɂ��邩�ǂ���"), SerializeField] bool isOnlyMoveToSide;
    [Tooltip("�ēx�G�ꂽ�Ƃ��Ɉړ����[�h�����Ƃɖ߂����ǂ���"), SerializeField] bool hasBackEffect;
    [Header("�@�\���L�����ǂ���"), SerializeField] bool isEffective;

    private void Awake()
    {
        isEffective = true;
        //���X�^�[�g���ɋ@�\��L���ɂ���
        GameManager.Instance.OnPlayerRestartObservable.Where(_ => !isEffective).Subscribe(_ => isEffective = true).AddTo(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        var playerMover = other.GetComponent<PlayerMover>();

        if(playerMover != null)
        {
            
            //�v���C���[�����X�N���[�����[�h�ŁA�ʏ�̈ړ����[�h�ɖ߂�@�\������Ȃ�A�G�ꂽ�v���C���[�̈ړ���ʏ탂�[�h�ɂ���
            if(playerMover.IsOnlyMoveToSide == isOnlyMoveToSide && hasBackEffect)
            {
                playerMover.SetIfOnlyMoveToSide(!isOnlyMoveToSide);
                isEffective = true;
            }

            //����ȊO�̏�Ԃł���ΐG�ꂽ�v���C���[�̈ړ������X�N���[�����[�h�ɕύX����
            else
            {
                if (isEffective)
                {
                    playerMover.SetIfOnlyMoveToSide(isOnlyMoveToSide);
                    //�v���C���[�𓹂̒����Ɉړ�������
                    other.transform.position = new Vector3(transform.position.x, other.transform.position.y, transform.position.z);

                    //�v���C���[��^���Ɍ�����
                    var forward = Quaternion.LookRotation(transform.forward);
                    other.transform.rotation = forward;

                    //�߂��Ă��čēx�G�ꂽ�Ƃ��ɂ�����x�������������Ȃ��悤��x�G�ꂽ��@�\�𖳌�������
                    isEffective = false;
                }
            }
        }
    }
}
