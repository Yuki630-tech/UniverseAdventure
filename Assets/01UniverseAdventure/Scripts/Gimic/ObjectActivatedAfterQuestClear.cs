using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ObjectActivatedAfterQuestClear : MonoBehaviour
{
    [Tooltip("�N�G�X�g�N���A��ɃA�N�e�B�u�ɂ���I�u�W�F�N�g"), SerializeField] GameObject objectActivatedAfterQuestClear;

    private void Awake()
    {
       
        //�v���C���[�����S���ă��X�^�[�g�������A����уN���A��ɍēx�Q�[�����v���C���n�߂��ۂɃI�u�W�F�N�g���\���B
        GameManager.Instance.OnPlayerRestartObservable.Where(_ => objectActivatedAfterQuestClear.activeSelf).Subscribe(_ => objectActivatedAfterQuestClear.SetActive(false));
        GameManager.Instance.OnResetGameObservable.Where(_ => objectActivatedAfterQuestClear.activeSelf).Subscribe(_ => objectActivatedAfterQuestClear.SetActive(false));
    }

    public void ChangeActiveState(bool setActiveState)
    {
        objectActivatedAfterQuestClear.SetActive(setActiveState);
    }

}
