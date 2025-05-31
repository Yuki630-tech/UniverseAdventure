using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movable:MonoBehaviour
{
    /// <summary>
    /// 移動可能にする
    /// </summary>
    public abstract void GoToMovable();

    /// <summary>
    /// Planetの切り替え時など特定のタイミングでオブジェクトの移動を不可能にする
    /// </summary>
    public abstract void GoToImmovable();
    
    /// <summary>
    /// 接地判定
    /// </summary>
    public abstract bool IsGround { get; }
}
