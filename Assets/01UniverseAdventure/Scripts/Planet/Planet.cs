using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Tooltip("���f�����ǂ���"), SerializeField] bool isSmall;

    /// <summary>
    /// ���f�����ǂ���
    /// </summary>
    public bool IsSmall { get => isSmall; }
}
