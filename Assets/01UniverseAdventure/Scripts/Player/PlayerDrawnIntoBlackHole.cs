using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDrawnIntoBlackHole : MonoBehaviour
{
    [Tooltip("初期の半径"), SerializeField] float startRadius = 10;
    [Tooltip("回転角度の初期値"), SerializeField] float startAngle = 0;
    [Tooltip("回転速度の初期値(+が右回り、-が左回り)"), SerializeField] float startRotateSpeed;
    [Tooltip("回転の最高速度(+が右回り、-が左回り)"), SerializeField] float maxRotateSpeed = 1;
    [Tooltip("回転の加速度(+が右回り、-が左回り。startrotateと符号を逆にすると減速)"), SerializeField] float rotateAcceleration;
    [Tooltip("半径を縮小させる速度"), SerializeField] float radiusSpeed;
    [Tooltip("回転の中心座標(初期)"), SerializeField] Vector3 center;
    [Tooltip("吸い込まれていく速度"), SerializeField] float drawnSpeed = 5;

    //吸い込まれていく間に更新されていく変数群
    float radius; 
    [SerializeField] float angle;
    float rotateSpeed;
    float posZ;
    /// <summary>
    /// 回転開始時の座標
    /// </summary>
    Vector3 startPos; 

    /// <summary>
    /// 回転の中心からブラックホールへの方向ベクトル(z軸)
    /// </summary>
    Vector3 directionFromCenterToBlackHole;

    /// <summary>
    /// プレイヤーから回転の中心への方向ベクトル(中心を決定するためのベクトル)
    /// </summary>
    Vector3 directionFromPlayerToCenter;
    /// <summary>
    /// 中心からプレイヤーへの方向ベクトル(x軸)
    /// </summary>
    Vector3 directionFromCenterToPlayer;

    /// <summary>
    /// y軸
    /// </summary>
    Vector3 directionY; 
    
    [Tooltip("吸い込まれる先となるブラックホールのtransform"), SerializeField] Transform targetTransform;

    public Vector3 Center { get => center; }
    public Vector3 DirectionFromCenterToBlackHole { get => directionFromCenterToBlackHole; }

    /// <summary>
    /// ブラックホールへの吸い込まれを開始する関数
    /// </summary>
    /// <param name="setTransform">吸い込まれる先となるblackhole</param>
    // Start is called before the first frame update
    public void StartGetDrawn(Transform setTransform)
    {
        targetTransform = setTransform;
        //それぞれの値を初期値に設定
        rotateSpeed = startRotateSpeed;
        radius = startRadius;
        angle = startAngle;
        posZ = 0;
        startPos = transform.position;

        //プレイヤーからブラックホールへの方向ベクトルとワールド空間の上方向との外積の方向にradiusの距離だけ進んだ位置を回転の中心として設定する
        var direction = targetTransform.position - startPos;
        direction = direction.normalized;
        directionFromPlayerToCenter = Vector3.Cross(direction, Vector3.up).normalized;
        center = startPos + directionFromPlayerToCenter * radius;

        //回転する際に利用する座標系についてx軸を中心からプレイヤーへの方向ベクトル、z軸を回転の中心からブラックホールへの方向ベクトルに設定する
        directionFromCenterToPlayer = transform.position - center;
        directionFromCenterToPlayer = directionFromCenterToPlayer.normalized; //x軸
        directionFromCenterToBlackHole = (targetTransform.position - center).normalized; //z軸

        //回転の座標系におけるz軸とx軸の外積をy軸とする
        directionY = Vector3.Cross(directionFromCenterToBlackHole, directionFromCenterToPlayer); //y軸

    }

    // Update is called once per frame
    public void UpdateGetDrawn()
    {
        GetDrawnMove();
        ChangeAngleZAndRadius();
        
    }

    /// <summary>
    /// 座標を更新する関数。
    /// </summary>

    void GetDrawnMove()
    {
         var x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
         var y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        　　　　　　　　　　　　　　　　　　//x軸                       //y軸                      //z軸
        transform.position = center + directionFromCenterToPlayer * x + directionY * y + directionFromCenterToBlackHole * posZ;
        DebugLog.Log("( " + x + " , " + y + " ) ");
    }

    /// <summary>
    /// 角度を毎秒回転速度だけ、回転速度を毎秒加速度だけ増加、半径を減少させブラックホールの方向に移動させる関数。
    /// </summary>

    void ChangeAngleZAndRadius()
    {
        angle += rotateSpeed * Time.deltaTime;
        rotateSpeed += rotateAcceleration * Time.deltaTime;
        rotateSpeed = Mathf.Clamp(rotateSpeed, 0, maxRotateSpeed);
        radius -= radiusSpeed * Time.deltaTime;
        radius = Mathf.Clamp(radius, 0, startRadius);

        posZ += drawnSpeed * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        if (targetTransform == null) return;
        var radius = startRadius;
        var startPos = transform.position;
        var direction = targetTransform.position - startPos;
        direction = direction.normalized;

        var directionToCenter = Vector3.Cross(direction, Vector3.up).normalized;
        var center = startPos + directionToCenter * radius;
        var directionFromCenterToPlayer = transform.position - center;
        directionFromCenterToPlayer = directionFromCenterToPlayer.normalized;
        var directionFromCenterToBlackHole = (targetTransform.position - center).normalized;
        var directionX = Vector3.Cross(directionFromCenterToPlayer, directionFromCenterToBlackHole);
        Gizmos.color = Color.green;
        if (targetTransform != null)
            Gizmos.DrawRay(startPos, direction * Vector3.Distance(startPos, targetTransform.position));

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(center, directionFromCenterToBlackHole * Vector3.Distance(targetTransform.position, center));
        Gizmos.color = Color.red;
        
        Gizmos.DrawRay(startPos, directionToCenter.normalized * radius);
        var wireSphereRadius = 0.5f;
        Gizmos.DrawWireSphere(center, wireSphereRadius);
        Gizmos.color = Color.black;
        Gizmos.DrawRay(center, directionX * radius);


    }
}
