using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class AudioPlayObj : MonoBehaviour
{
    [Tooltip("�v���C���[���ʉ߂������ɖ炷AudioClip�̖��O"), SerializeField] AudioClipData.AudioClipName clipName;
    [Header("�v���C���[���ʉ߂�������Audio��؂�ւ��čĐ����邩�ǂ���"), SerializeField] bool isEffective;

    private void Awake()
    {
        isEffective = true;
        //�v���C���[�����X�^�[�g�����ۂɂ��̃I�u�W�F�N�g��L���ɂ��ăv���C���[���G�ꂽ�Ƃ���audio���؂�ւ��悤�ɂ���
        GameManager.Instance?.OnPlayerRestartObservable.Where(_ => !isEffective).Subscribe(_ => isEffective = true).AddTo(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!isEffective) { return; }
        if (other.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance?.PlayAudio(clipName);
            //�v���C���[���߂��Ă��čēx�G�ꂽ�Ƃ���audio���s���R�ɐ؂�ւ��Ȃ��悤�ɋ@�\�𖳌������Ă���
            isEffective = false;

        }
    }
}
