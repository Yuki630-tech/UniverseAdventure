using Cinemachine;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EraserWithDownsize : MonoBehaviour
{
    [Tooltip("消滅させるオブジェクトのリスト"), SerializeField] private List<GameObject> eraseObjs = new List<GameObject>();
    [Tooltip("有効にするカメラ"), SerializeField] private CinemachineVirtualCamera blackHoleCamera;
    [Tooltip("プレイヤーカメラに戻すまでの間隔"), SerializeField] private float interval = 2f;
    [Tooltip("戻すさきのプレイヤーカメラ"), SerializeField] private CinemachineVirtualCamera playerCamera;
    [Tooltip("縮小速度"), SerializeField] private float downsizeSpeed = 0.8f;
   
    /// <summary>
    /// BlackHoleクラッシャーを取った時のUnityEventとして処理
    /// </summary>
    public void EraseObject()
    {
        EraseObjectTask().Forget();
    }

    private async UniTaskVoid EraseObjectTask()
    {
        CameraManager.Instance.ChangeCamera(blackHoleCamera);
        await DownsizeTask();
        await UniTask.Delay(TimeSpan.FromSeconds(interval));
        CameraManager.Instance.ChangeCamera(playerCamera);
    }

    private async UniTask DownsizeTask()
    {
        var tasks = new List<UniTask>();
        foreach (var obj in eraseObjs)
        {
            tasks.Add(Downsize(obj));
        }

        await UniTask.WhenAll(tasks);
    }

    private async UniTask Downsize(GameObject obj)
    {
        while (obj.transform.localScale.x >= 0f)
        {
            Vector3 downsize = new Vector3(downsizeSpeed, downsizeSpeed, downsizeSpeed);
            obj.transform.localScale -= downsize;
            await UniTask.Yield();

        }
        obj.SetActive(false);

    }
}
