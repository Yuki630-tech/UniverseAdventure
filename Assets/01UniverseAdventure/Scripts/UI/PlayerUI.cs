using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using DG.Tweening;

public class PlayerUI : MonoBehaviour
{
    [Tooltip("�v���C���[��hp��\�����郉�C�t�Q�[�W�̃Z�b�g"), SerializeField] GameObject lifeGauge;
    [Tooltip("�v���C���[��hp�̒l���������C�t�Q�[�W"), SerializeField] Image lifeGauge1;
    [Tooltip("hp�e�L�X�g"), SerializeField] TextMeshProUGUI lifeText;
    [Tooltip("�X�g�b�N��\������e�L�X�g"), SerializeField] TextMeshProUGUI stockText;
    [Tooltip("��̃��C�t�o�[�ŕ\����hp�̍ő�l"), SerializeField] float defaultMaxHp = 3;
    [Tooltip("�v���C���[�̊e�p�����[�^���擾���邽��playerStatus���擾���Ă���"), SerializeField] PlayerStatus playerStatus;
    [Tooltip("���C�t1�̎����C�t�o�[��U�������鎞��"), SerializeField] float shakeDuration = 0.5f;
    [Tooltip("���C�t�o�[��U��������Ԋu"), SerializeField] float shakeInterval = 1f;
    [Tooltip("���C�t�o�[�̐U�����̐U��"), SerializeField] float shakeStrength = 3f;
    Tweener shakeTween;
    Coroutine shakeCoroutine;


    private void Awake()
    {
        var initPos = lifeGauge1.rectTransform.position;
        playerStatus.Hp.Subscribe(hp => lifeGauge1.fillAmount = hp / defaultMaxHp).AddTo(gameObject);

        //hp��3�̎��̓��C�t�o�[�̐F��΁A2�̎��͉��F�A1�̎��͐ԐF�ɂ���
        playerStatus.Hp.Where(hp => hp == 3).Subscribe(_ => lifeGauge1.color = Color.green).AddTo(gameObject);

        playerStatus.Hp.Where(hp => hp == 2).Subscribe(_ =>
        {
            lifeGauge1.color = Color.yellow;
        }).AddTo(gameObject);

        playerStatus.Hp.Where(hp => hp == 1).Subscribe(_ =>
        {
            lifeGauge1.color = Color.red;
            shakeCoroutine = StartCoroutine(ShakeLifeGauge());//�U��������
        }).AddTo(gameObject);

        playerStatus.Hp.Where(hp => hp >= 2 && shakeCoroutine != null).Subscribe(_ =>
        {
            StopCoroutine(shakeCoroutine); //�U�����~�߂�
            shakeTween?.Kill();
            lifeGauge.transform.position = initPos;
        }).AddTo(gameObject);

        playerStatus.Hp.Subscribe(hp => lifeText.text = hp.ToString()).AddTo(gameObject); //hp�\��

        playerStatus.Stock.Subscribe(stock => stockText.text = stock.ToString()).AddTo(gameObject); //stock�\��
    }

    /// <summary>
    /// ���C�t�o�[��U��������R���[�`��
    /// </summary>
    /// <returns></returns>
    IEnumerator ShakeLifeGauge()
    {
        while (playerStatus.Hp.Value == 1)
        {
            var lifeGaugeTransform = lifeGauge.transform;
            shakeTween = lifeGauge.transform.DOShakePosition(shakeDuration, shakeStrength); //DOTween��DOShakePosition���g���ĐU��������
            yield return new WaitForSeconds(shakeInterval);
            shakeTween = null;
        }
    }
}
