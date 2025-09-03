using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Convert
{
    public static Vector3 ConvertToWorld(Vector3 pivot, Transform baseTrans, Vector3 localPos)
    {
        return pivot + localPos.x * baseTrans.right + localPos.y * baseTrans.up + localPos.z * baseTrans.forward;
    }
}
