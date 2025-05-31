using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CustomGravityChanger : MonoBehaviour
{
    [Tooltip("�G�ꂽ�Ƃ��Ƀv���C���[�𓮂��Ȃ��悤�ɂ��邩�ǂ���"), SerializeField] bool hasImmovableEffect;
    [Tooltip("�v���C���[���ڒn����܂Ŏ��̏d�͐؂�ւ����N����Ȃ��悤�ɂ��邩�ǂ���"), SerializeField] bool hasWaitUntilGroundedEffect;

    /// <summary>
    /// �@�\���L�����ǂ���
    /// </summary>
    bool isEffective;
    private void Awake()
    {
        isEffective = true;
        //�v���C���[�����S���ă��X�^�[�g�������ēx�@�\��L���ɂ���
        GameManager.Instance?.OnPlayerRestartObservable.Where(_ => !isEffective).Subscribe(_ => isEffective = true).AddTo(gameObject);
    }

    private async void OnTriggerEnter(Collider other)
    {
        var gravity = other.GetComponent<Gravity>();
        if(gravity != null && isEffective)
        {
            
            //���g�̏������G�ꂽ����̏d�͂̏�����ɐݒ肷��B
            await gravity.SetGravity(gameObject, hasImmovableEffect, hasWaitUntilGroundedEffect, transform.up);
            isEffective = false;
        }
    }
}
