using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjMover : MonoBehaviour
{
    [Tooltip("移動させる対象のオブジェクトのトランスフォーム"), SerializeField] private Transform moveTrans;
    [Tooltip("移動させる目的地"), SerializeField] private Transform targetTrans;
    [Tooltip("ターゲットのほうに向く際の回転速度"), SerializeField] private float rotateSpeed;
    
    public void MoveToTargetPos()
    {
        moveTrans.position = targetTrans.position;
        moveTrans.rotation = targetTrans.rotation;
    }

    public void RotateToTargetPos()
    {
        var direction = (targetTrans.position - moveTrans.position).normalized;
        var gravity = moveTrans.GetComponent<Gravity>();
        var right = Vector3.Cross(gravity.NormalVec, direction).normalized;
        var forward = Vector3.Cross(right, gravity.NormalVec).normalized;
        var rot = Quaternion.LookRotation(forward, moveTrans.up);
        StartCoroutine(RotateToTargetPosCoroutine(rot));
    }

    IEnumerator RotateToTargetPosCoroutine(Quaternion setForward)
    {
        while(moveTrans.rotation != setForward)
        {
            moveTrans.rotation = Quaternion.RotateTowards(moveTrans.rotation, setForward, rotateSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
