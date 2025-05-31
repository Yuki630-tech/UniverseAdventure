using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundTripObj : MonoBehaviour
{
    [Tooltip("一つ目の目的地"), SerializeField] Transform destination1Trans;
    [Tooltip("二つ目の目的地"), SerializeField] Transform destination2Trans;
    [Tooltip("移動速度"), SerializeField] float moveSpeed;
    Vector3 destination1Pos;
    Vector3 destination2Pos;

    /// <summary>
    /// 目的地2に向かっている場合はtrue。目的地1に向かっている場合はfalse。
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
    /// 往復運動を行う関数
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
    /// フラグを切り替える関数
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
