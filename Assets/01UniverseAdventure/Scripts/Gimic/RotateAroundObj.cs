using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundObj : MonoBehaviour
{
    [Tooltip("��]�̒��S�ƂȂ�I�u�W�F�N�g"), SerializeField] Transform centerTrans;
    [Tooltip("��]���鑬��"), SerializeField] float rotateSpeed;
    [Tooltip("��]�̔��a"), SerializeField] float radius;

    /// <summary>
    /// ��]�p�x
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
    /// �I�u�W�F�N�g������I�u�W�F�N�g����ɉ�]������֐�
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
