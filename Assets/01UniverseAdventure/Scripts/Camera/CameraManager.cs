using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class CameraManager : MonoBehaviour
{
    [Header("シーン上にある全てのvirtualcameraのリスト")]public List<CinemachineVirtualCamera> cinemachineVirtualCameras = new List<CinemachineVirtualCamera>();
    [Tooltip("プレイヤーのカメラ"),SerializeField] PlayerCamera playerCamera;

    public static CameraManager Instance { get; private set; }
    [Tooltip("シーン上にあるメインカメラ"), SerializeField] Camera mainCamera;
    [Tooltip("メインカメラにセットされているCinemachineBrain"), SerializeField] CinemachineBrain mainCameraBrain;
    [Header("チェックポイントに触れたときに有効になっているカメラの名前"), SerializeField] string cameraNameBeforeDie;

    private void Awake()
    {
        //シングルトン
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }

        //プレイヤーがリスポンした際にブラックホール演出のため無効化されたmainCameraBrainを有効化させる
        //カメラをチェックポイントにいたときのものに切り替える
        //povのそれぞれの軸の値を0にする
        GameManager.Instance?.OnPlayerRestartObservable.Where(_ => !mainCameraBrain.enabled).Subscribe(_ =>
        {
            mainCameraBrain.enabled = true; 
        }
        ).AddTo(this);

        GameManager.Instance?.OnPlayerRestartObservable.Subscribe(_ =>
        {
            var pov = GetCamera("PlayerVirtualCamera").GetCinemachineComponent<CinemachinePOV>();
            if (pov != null)
            {
                pov.m_HorizontalAxis.Value = 0f;
                pov.m_VerticalAxis.Value = 0f;
            }
            ChangeCamera(cameraNameBeforeDie, CinemachineBlendDefinition.Style.Cut, 0f);
        });
        
    }

    /// <summary>
    /// プレイヤーカメラを登録する関数
    /// </summary>
    /// <param name="setCamera"></param>
    public void SetPlayerCamera(PlayerCamera setCamera)
    {
        playerCamera = setCamera;
    }

    /// <summary>
    /// メインカメラを登録する関数
    /// </summary>

    public void SetMainCamera()
    {
        if(mainCamera == null)
        {
            mainCamera = Camera.main;
            mainCameraBrain = mainCamera.GetComponent<CinemachineBrain>();
        }
       
    }

    /// <summary>
    /// 指定したカメラに切り替える関数
    /// </summary>
    /// <param name="cameraName"></param>
    /// <param name="blendStyle"></param>
    /// <param name="blendTime"></param>

    public void ChangeCamera(string cameraName, CinemachineBlendDefinition.Style blendStyle = CinemachineBlendDefinition.Style.EaseInOut, float blendTime = 2.0f)
    {
        //現在有効になっているカメラと切り替えるカメラをシーン上から取得
        var camera1 = cinemachineVirtualCameras.FirstOrDefault(camera => camera.Priority == 10);
        var camera2 = cinemachineVirtualCameras.FirstOrDefault(camera => camera.Name == cameraName);

        //それぞれのPriorityを変えることでカメラを切り替え
        camera1.Priority = -1;
        camera2.Priority = 10;
        SetBlend(blendStyle, blendTime);

        //惑星に沿ってカメラの親オブジェクトを回転させているので横スクロールモードになった時に変な回転状態にならないように回転状態を初期化する
        if (cameraName == "PlayerVirtualCameraFromSide")
        {
            playerCamera.InitializeRot();
        }

    }

    /// <summary>
    /// ブレンドモードを設定する関数
    /// </summary>
    /// <param name="blendStyle"></param>
    /// <param name="blendTime"></param>
    void SetBlend(CinemachineBlendDefinition.Style blendStyle, float blendTime)
    {
        //ブレンドタイプとタイムを設定するためにCinemachineBlendDefinitionを作成。それぞれの値を引数で指定したものに設定。
        var blend = new CinemachineBlendDefinition();

        blend.m_Style = blendStyle;
        blend.m_Time = blendTime;

        //メインカメラのシネマシーンブレインに作成したCinemachineBlendDefenitionを設定
        mainCameraBrain.m_DefaultBlend = blend;

    }
    /// <summary>
    /// 現在有効なカメラを取得する関数
    /// </summary>
    /// <returns></returns>

    public CinemachineVirtualCamera GetCurrentCamera()
    {
        return cinemachineVirtualCameras.FirstOrDefault(camera => camera.Priority == 10);
    }

    /// <summary>
    /// 指定したカメラを取得する関数
    /// </summary>
    /// <param name="cameraName"></param>
    /// <returns></returns>
    public CinemachineVirtualCamera GetCamera(string cameraName)
    {
        return cinemachineVirtualCameras.FirstOrDefault(camera => camera.Name == cameraName);
    }

    /// <summary>
    /// カメラを指定したポジションに移動させる関数(cinemachineを使わない演出)
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="moveSpeed"></param>
    /// <returns></returns>

    public IEnumerator MoveCameraTo(Vector3 destination, float moveSpeed)
    {
        mainCameraBrain.enabled = false;
        while (Vector3.Distance(mainCamera.transform.position, destination) > 0.01f)
        {
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, destination, moveSpeed);
            yield return null;
        }

        mainCamera.transform.position = destination;
    }

    /// <summary>
    /// カメラをしていたローテーションに回転させる関数(cinemachineを使わない演出)
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="rotateSpeed"></param>
    /// <returns></returns>

    public IEnumerator RotateCameraTo(Quaternion destination, float rotateSpeed)
    {
        mainCameraBrain.enabled = false;
        while (Quaternion.Angle(mainCamera.transform.rotation, destination) > 0.01f)
        {
            mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, destination, rotateSpeed);
            yield return null;
        }

        mainCamera.transform.rotation = destination;
    }
    /// <summary>
    /// リスポーンした時にカメラを戻すためチェックポイントに触れたときのカメラの名前を登録する関数
    /// </summary>
    public void SetCameraBeforeDie()
    {
        cameraNameBeforeDie = Instance.GetCurrentCamera().gameObject.name;
    }
}
