using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DirectionMakerOnSphere
{
    [Header("自分自身のTransform"), SerializeField] private Transform ownerTrans;
    [Header("惑星の法線を取得するためのGravityコンポーネント"), SerializeField] private Gravity gravity;

    public DirectionMakerOnSphere(Transform setOwnerTrans, Gravity setGravity)
    {
        ownerTrans = setOwnerTrans;
        gravity = setGravity;
    }
    public Vector3 MakeVector(Vector3 setDestination)
    {
        Vector3 direction = (setDestination - ownerTrans.position).normalized;
        Vector3 right = Vector3.Cross(gravity.NormalVec, direction);
        Vector3 forward = Vector3.Cross(right, gravity.NormalVec);
        return forward;
    }
}
