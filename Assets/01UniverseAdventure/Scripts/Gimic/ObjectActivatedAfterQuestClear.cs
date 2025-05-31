using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ObjectActivatedAfterQuestClear : MonoBehaviour
{
    [Tooltip("クエストクリア後にアクティブにするオブジェクト"), SerializeField] GameObject objectActivatedAfterQuestClear;

    private void Awake()
    {
       
        //プレイヤーが死亡してリスタートした時、およびクリア後に再度ゲームをプレイし始めた際にオブジェクトを非表示。
        GameManager.Instance.OnPlayerRestartObservable.Where(_ => objectActivatedAfterQuestClear.activeSelf).Subscribe(_ => objectActivatedAfterQuestClear.SetActive(false));
        GameManager.Instance.OnResetGameObservable.Where(_ => objectActivatedAfterQuestClear.activeSelf).Subscribe(_ => objectActivatedAfterQuestClear.SetActive(false));
    }

    public void ChangeActiveState(bool setActiveState)
    {
        objectActivatedAfterQuestClear.SetActive(setActiveState);
    }

}
