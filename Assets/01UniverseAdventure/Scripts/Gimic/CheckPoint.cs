using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    bool isEffective;
    private void Awake()
    {
        isEffective = true;
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isEffective)
        {
            
            //ゲームがリスタートしたときにこの場所に戻ってくるようにこのオブジェクトの座標をリスポーン地に設定する
            GameManager.Instance.SetRestartPos(transform.position);

            //ゲームがリスタートした時のbgmとカメラにそれぞれ現在有効になっているものを登録しておく
            AudioManager.Instance.SetAudioBeforeDie();
            CameraManager.Instance.SetCameraBeforeDie();

            CameraManager.Instance.SetMainCamera();
            isEffective = false;
        }
    }
}
