using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SetMoveModeObject : MonoBehaviour
{
    [Tooltip("プレイヤーが触れたときにプレイヤーを横スクロールモードにするかどうか"), SerializeField] bool isOnlyMoveToSide;
    [Tooltip("再度触れたときに移動モードをもとに戻すかどうか"), SerializeField] bool hasBackEffect;
    [Header("機能が有効かどうか"), SerializeField] bool isEffective;

    private void Awake()
    {
        isEffective = true;
        //リスタート時に機能を有効にする
        GameManager.Instance.OnPlayerRestartObservable.Where(_ => !isEffective).Subscribe(_ => isEffective = true).AddTo(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        var playerMover = other.GetComponent<PlayerMover>();

        if(playerMover != null)
        {
            
            //プレイヤーが横スクロールモードで、通常の移動モードに戻る機能があるなら、触れたプレイヤーの移動を通常モードにする
            if(playerMover.IsOnlyMoveToSide == isOnlyMoveToSide && hasBackEffect)
            {
                playerMover.SetIfOnlyMoveToSide(!isOnlyMoveToSide);
                isEffective = true;
            }

            //それ以外の状態であれば触れたプレイヤーの移動を横スクロールモードに変更する
            else
            {
                if (isEffective)
                {
                    playerMover.SetIfOnlyMoveToSide(isOnlyMoveToSide);
                    //プレイヤーを道の中央に移動させる
                    other.transform.position = new Vector3(transform.position.x, other.transform.position.y, transform.position.z);

                    //プレイヤーを真横に向ける
                    var forward = Quaternion.LookRotation(transform.forward);
                    other.transform.rotation = forward;

                    //戻ってきて再度触れたときにもう一度同じ処理をしないよう一度触れたら機能を無効化する
                    isEffective = false;
                }
            }
        }
    }
}
