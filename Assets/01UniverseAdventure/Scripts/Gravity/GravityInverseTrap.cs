using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityInverseTrap : MonoBehaviour
{
    [Tooltip("�I�u�W�F�N�g���G�ꂽ�Ƃ��ɓ������Ȃ��悤�ɂ��邩�ǂ���"), SerializeField] bool hasImmovableEffect;
    [Tooltip("�I�u�W�F�N�g���G�ꂽ��ڒn����܂ŋ@�\���~���邩�ǂ���"), SerializeField] bool hasWaitUntilGroundedEffect;
    private async void OnTriggerEnter(Collider other)
    {
        var gravity = other.GetComponent<Gravity>();
        if (gravity == null) return;
        await gravity.SetGravity(gameObject, hasImmovableEffect, hasWaitUntilGroundedEffect, -transform.up);

    }
}
