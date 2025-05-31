using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackData
{
    [Tooltip("あたえるダメージの量"), SerializeField] int damage = 1;
    [Tooltip("吹き飛ばす向き"), SerializeField] Vector3 knockBack;

    /// <summary>
    /// 与えるダメージの量
    /// </summary>
    public int Damage { get => damage; }

    /// <summary>
    /// 吹き飛ばす向き
    /// </summary>
    public Vector3 KnockBack { get => knockBack; }
}
