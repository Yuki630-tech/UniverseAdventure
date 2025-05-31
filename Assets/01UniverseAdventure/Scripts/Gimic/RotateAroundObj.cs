using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundObj : MonoBehaviour
{
    [Tooltip("回転の中心となるオブジェクト"), SerializeField] Transform centerTrans;
    [Tooltip("回転する速さ"), SerializeField] float rotateSpeed;
    [Tooltip("回転の半径"), SerializeField] float radius;

    /// <summary>
    /// 回転角度
    /// </summary>

    float angle;

    // Start is called before the first frame update
    void Start()
    {
        angle = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        RotateAround();
    }

    /// <summary>
    /// オブジェクトをあるオブジェクト周りに回転させる関数
    /// </summary>
    void RotateAround()
    {
        angle += rotateSpeed * Time.deltaTime;
        var x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        var z = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        var pos = new Vector3(x, 0f, z);

        transform.position = centerTrans.position + pos;
    }

}
