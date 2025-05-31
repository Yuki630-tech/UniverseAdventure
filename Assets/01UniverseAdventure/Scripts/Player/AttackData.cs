using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackData
{
    [Tooltip("��������_���[�W�̗�"), SerializeField] int damage = 1;
    [Tooltip("������΂�����"), SerializeField] Vector3 knockBack;

    /// <summary>
    /// �^����_���[�W�̗�
    /// </summary>
    public int Damage { get => damage; }

    /// <summary>
    /// ������΂�����
    /// </summary>
    public Vector3 KnockBack { get => knockBack; }
}
