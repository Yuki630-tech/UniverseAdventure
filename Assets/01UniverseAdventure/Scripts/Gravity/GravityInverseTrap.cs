using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityInverseTrap : MonoBehaviour
{
    [Tooltip("オブジェクトが触れたときに動かさないようにするかどうか"), SerializeField] bool hasImmovableEffect;
    [Tooltip("オブジェクトが触れた後接地するまで機能を停止するかどうか"), SerializeField] bool hasWaitUntilGroundedEffect;
    [Tooltip("PlayerCamera(地面に沿ってカメラを回転させるか決めるために"), SerializeField] PlayerCamera playerCamera;
    [Tooltip("カメラを地面に沿って回転させるかどうか"), SerializeField] private bool isEnableToRotateWithGravity;
    private async void OnTriggerEnter(Collider other)
    {
        var gravity = other.GetComponent<Gravity>();
        if (gravity == null) return;
        playerCamera.SetIfEnableToFlip(isEnableToRotateWithGravity);
        await gravity.SetGravity(gameObject, hasImmovableEffect, hasWaitUntilGroundedEffect, -transform.up);

    }
}
