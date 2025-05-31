using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using Cysharp.Threading.Tasks;

public class SetGravityChanger : MonoBehaviour
{
    [Tooltip("�d�͐؂�ւ��̊J�n�Ȃǂ��̃I�u�W�F�N�g�Ƀv���C���[���G�ꂽ�ۂ̃C�x���g��o�^"), SerializeField] UnityEvent gravityChangeEvent;
    [Tooltip("�v���C���[�ɓo�^����gravityChanger"), SerializeField] GravityChanger gravityChanger;
    [SerializeField] bool isEffective;

    private void Awake()
    {
        isEffective = true;
        //�v���C���[�����S���ăQ�[�������X�^�[�g�����ۂɋ@�\���L���ɂȂ�悤�ɂ���
        GameManager.Instance?.OnPlayerRestartObservable.Subscribe(_ => isEffective = true).AddTo(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        //�@�\���L���Ȃ�ΐݒ肵��gravityChanger���v���C���[��gravity�ɓo�^����B
        //�܂��v���C���[���߂��Ă��čēx�G�ꂽ�Ƃ��ɏd�͐؂�ւ��̃R���[�`������d�Ɏn�܂�Ȃ��悤�Ɉ�x�G�ꂽ��@�\�𖳌�������
        if (other.CompareTag("Player") && isEffective)
        {
            var gravity = other.GetComponent<Gravity>();

            gravity.SetGravityChanger(gravityChanger);
            gravityChangeEvent?.Invoke();
            isEffective = false;
        }
    }
}
