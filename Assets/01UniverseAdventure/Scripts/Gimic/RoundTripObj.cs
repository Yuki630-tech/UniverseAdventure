using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundTripObj : MonoBehaviour
{
    [Tooltip("��ڂ̖ړI�n"), SerializeField] Transform destination1Trans;
    [Tooltip("��ڂ̖ړI�n"), SerializeField] Transform destination2Trans;
    [Tooltip("�ړ����x"), SerializeField] float moveSpeed;
    Vector3 destination1Pos;
    Vector3 destination2Pos;

    /// <summary>
    /// �ړI�n2�Ɍ������Ă���ꍇ��true�B�ړI�n1�Ɍ������Ă���ꍇ��false�B
    /// </summary>
    bool flag;
    // Start is called before the first frame update
    void Start()
    {
        destination1Pos = destination1Trans.position;
        destination2Pos = destination2Trans.position;
    }

    // Update is called once per frame
    void Update()
    {
        OperateFlag();

        MakeARoundTrip();
    }

    /// <summary>
    /// �����^�����s���֐�
    /// </summary>
    void MakeARoundTrip()
    {
        if (flag)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination2Pos, moveSpeed * Time.deltaTime);
        }

        else
        {
            transform.position = Vector3.MoveTowards(transform.position, destination1Pos, moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// �t���O��؂�ւ���֐�
    /// </summary>
    void OperateFlag()
    {
        if(Vector3.Distance(transform.position, destination1Pos) <= 0.01f)
        {
            flag = true;
        }

        else if (Vector3.Distance(transform.position, destination2Pos) <= 0.01f)
        {
            flag = false;   
        }
    }
}
