using Cinemachine;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using DG;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour
{
    [Tooltip("惑星の垂直方向への回転スピード"), SerializeField] float rotateSpeed;
    [Tooltip("スターリングに乗っているときのプレイヤーの上方向への回転スピード"), SerializeField] float rotateSpeedWhileFlying;
    [Tooltip("惑星の垂直方向を検知するために登録するプレイヤーのGravity"), SerializeField] Gravity playerGravity;
    [Tooltip("プレイヤーがスターリングに乗った状態かどうか"), SerializeField] bool isFlying;
    [Tooltip("プレイヤー用のVirtualCamera。小惑星上で障害物で遮られた時にpovの値を変えるために保持"), SerializeField] CinemachineVirtualCamera playerVirtualCamera;
    [Tooltip("障害物とみなすレイヤー"), SerializeField] LayerMask layerMask;
    [Tooltip("垂直とみなす内積"), SerializeField] float verticalDot = 0f;
    [Tooltip("平行とみなす内積"), SerializeField] float parallelDot = 0.98f;
    [Tooltip("プレイヤーが下に向かう時のpovの垂直軸の値"), SerializeField] float downPovVerticalValue = -45f;
    [Tooltip("プレイヤーが上に向かう時のpovの垂直軸の値"), SerializeField] float upPovVerticalValue = 45f;
    [Tooltip("pov値を操作するDOTweenのduration"), SerializeField] float durationOfPovChange = 1f;

    /// <summary>
    /// プレイヤーが小惑星上にいるかどうか
    /// </summary>
    bool isOnSmallPlanet;
    [SerializeField]
    ReactiveProperty<bool> isNotOnNormalPlanet = new ReactiveProperty<bool>();

    CinemachinePOV pov;
    enum PlayerPos
    {
        up,
        down
    }

    PlayerPos playerPos;

    /// <summary>
    /// 初期の回転値
    /// </summary>
    Quaternion startRot;

    private void Awake()
    {
        playerPos = PlayerPos.up;
        //初期回転を取得
        startRot = transform.rotation;

        //カメラマネージャーに自身をPlayerCameraとして登録→カメラマネージャーからInitializeRot()を呼び出せるようにし、
        //横スクロールモードになった時に回転値を初期値に戻せるようにするため。
        CameraManager.Instance?.SetPlayerCamera(this);
        isNotOnNormalPlanet.Where(isNotOnNormalPlanet => isNotOnNormalPlanet).Subscribe(_ =>
        {
            var bodyUp = transform.up;
            var up = Vector3.up;
            transform.rotation = Quaternion.FromToRotation(bodyUp, up) * transform.rotation;
            
        });

    }
    private void Update()
    {
        isOnSmallPlanet = playerGravity.Planet != null && playerGravity.Planet.IsSmall;
        isNotOnNormalPlanet.Value = (playerGravity.PlanetObj == null || isOnSmallPlanet) && !isFlying;
        //スターリングに乗っているときにはプレイヤーの上方向、惑星上にいるときはgravity.NormalVec方向にカメラを回転させる
        if (isFlying)
        {
            SetCameraRotWhileFlying();
        }

        else
        {
            SetCameraRot();
        }

    }
    /// <summary>
    /// 回転値を初期値に戻す関数
    /// </summary>

    public void InitializeRot()
    {
        transform.rotation = startRot;
    }
    #region Planet
    /// <summary>
    /// カメラの上方向が惑星の垂直方向に向くように回転させる関数。惑星上にいないときにはワールド座標の上方向にカメラの上方向を向かせる
    /// </summary>

    void SetCameraRot()
    {
        var bodyUp = transform.up;
        //惑星上にいる場合惑星のNormalVecにカメラの上方向が向くようにする
        if (playerGravity.PlanetObj != null && !isOnSmallPlanet)
        {
            var playerUp = playerGravity.NormalVec;

            var rotation = Quaternion.FromToRotation(bodyUp, playerUp) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed * Time.deltaTime);
        }

    }
    #endregion

    #region StarRing
    /// <summary>
    /// スターリングに乗っているときにカメラの上方向がプレイヤーの上方向を向くように回転させる関数。
    /// </summary>
    void SetCameraRotWhileFlying()
    {
        var bodyUp = transform.up;
        var playerUp = playerGravity.transform.up;

        var rotation = Quaternion.FromToRotation(bodyUp, playerUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeedWhileFlying * Time.deltaTime);
    }

    /// <summary>
    /// プレイヤーがスターリングに乗っているかどうかを登録する関数
    /// </summary>
    /// <param name="setFlying"></param>
    public void SetFlying(bool setFlying)
    {
        isFlying = setFlying;
    }
    #endregion

    

    #region Dotween
    /// <summary>
    /// DOTweenのアニメーションをするために初期値を取得する関数
    /// </summary>
    /// <returns></returns>
    float GetCurrentPovValue() => pov.m_VerticalAxis.Value;

    void SetCurrentPovValue(float value)
    {
        pov.m_VerticalAxis.Value = value;
    }
    #endregion
}

