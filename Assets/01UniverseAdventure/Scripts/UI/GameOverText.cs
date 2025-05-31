using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Cysharp.Threading.Tasks;

public class GameOverText : MonoBehaviour
{
    [Tooltip("���g��rectTransform"), SerializeField] RectTransform rectTransform;
    [Tooltip("DOTween�Đ��O�̃e�L�X�g�̏����ʒu"), SerializeField] Vector3 startPos;
    [Tooltip("DOTween�̃o�E���h�������鎞��"), SerializeField] float durationToBounce;
    [Tooltip("x�����̃C�[�W���O"), SerializeField] Ease textEaseX;
    [Tooltip("y�����̃C�[�W���O")]
    [SerializeField] Ease textEaseY;
    private void OnEnable()
    {
        //�C���X�y�N�^�[�Őݒ肵���ʒu���e�L�X�g�̃S�[���n�_�Ƃ���
        var endPos = rectTransform.position;
        //startPos�����ړ�
        rectTransform.position += startPos;

        //�C�[�W���O�ɏ]����DOTween�A�j���[�V����
        rectTransform.DOMoveX(endPos.x, durationToBounce).SetEase(textEaseX);
        rectTransform.DOMoveY(endPos.y, durationToBounce).SetEase(textEaseY);
    }
}
