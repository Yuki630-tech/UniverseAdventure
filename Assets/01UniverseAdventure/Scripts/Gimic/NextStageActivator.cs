using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextStageActivator : MonoBehaviour
{
    [Tooltip("�v���C���[���G�ꂽ�Ƃ��ɏo��������X�e�[�W"), SerializeField] GameObject nextStage;

    private void Awake()
    {
        //nextStage.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nextStage.SetActive(true);
        }
    }
}
