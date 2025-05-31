using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using Cysharp.Threading.Tasks;

public class SetGravityChanger : MonoBehaviour
{
    [Tooltip("重力切り替えの開始などこのオブジェクトにプレイヤーが触れた際のイベントを登録"), SerializeField] UnityEvent gravityChangeEvent;
    [Tooltip("プレイヤーに登録するgravityChanger"), SerializeField] GravityChanger gravityChanger;
    [SerializeField] bool isEffective;

    private void Awake()
    {
        isEffective = true;
        //プレイヤーが死亡してゲームがリスタートした際に機能が有効になるようにする
        GameManager.Instance?.OnPlayerRestartObservable.Subscribe(_ => isEffective = true).AddTo(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        //機能が有効ならば設定したgravityChangerをプレイヤーのgravityに登録する。
        //またプレイヤーが戻ってきて再度触れたときに重力切り替えのコルーチンが二重に始まらないように一度触れたら機能を無効化する
        if (other.CompareTag("Player") && isEffective)
        {
            var gravity = other.GetComponent<Gravity>();

            gravity.SetGravityChanger(gravityChanger);
            gravityChangeEvent?.Invoke();
            isEffective = false;
        }
    }
}
