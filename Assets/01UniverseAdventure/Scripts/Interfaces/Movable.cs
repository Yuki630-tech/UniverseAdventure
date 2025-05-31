using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movable:MonoBehaviour
{
    /// <summary>
    /// �ړ��\�ɂ���
    /// </summary>
    public abstract void GoToMovable();

    /// <summary>
    /// Planet�̐؂�ւ����ȂǓ���̃^�C�~���O�ŃI�u�W�F�N�g�̈ړ���s�\�ɂ���
    /// </summary>
    public abstract void GoToImmovable();
    
    /// <summary>
    /// �ڒn����
    /// </summary>
    public abstract bool IsGround { get; }
}
