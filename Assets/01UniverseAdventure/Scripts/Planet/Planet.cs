using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Tooltip("小惑星かどうか"), SerializeField] bool isSmall;

    /// <summary>
    /// 小惑星かどうか
    /// </summary>
    public bool IsSmall { get => isSmall; }
}
