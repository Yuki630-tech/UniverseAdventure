using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using Cysharp.Threading.Tasks;

public class DieScene : MonoBehaviour
{
    [Tooltip("gameOver�̃e�L�X�g"), SerializeField] GameObject gameOverText;
    PlayerStatus playerStatus; //�X�g�b�N��\�������邽�߂Ƀv���C���[�̃X�e�[�^�X�����擾
    [Tooltip("�v���C���[������ł���gameOver�e�L�X�g��\��������܂ł̎���"), SerializeField] float waitTimeFromDieToAppearText = 1;
    [Tooltip("gameOver�e�L�X�g���\������Ă���t�F�C�h�A�E�g����܂ł̎���"), SerializeField] float waitTimeUntilDieTextFade = 1;
    [Tooltip("gameOver�e�L�X�g����\���ɂȂ��Ă���X�g�b�N�e�L�X�g���\�������܂ł̎���"), SerializeField] float waitTimeFromDieTextToStockText = 1f;
    [Tooltip("�X�g�b�N�e�L�X�g���\������Ă���X�g�b�N�����炷�܂ł̎���"), SerializeField] float waitTimeFromStockTextToRemoveStock = 1f;
    [Tooltip("���ҋ@�̎���(�e�L�X�g���\������Ă���u���b�N�A�E�g���J�n����܂łȂǒZ���ҋ@)"), SerializeField] float waitTimeFromAppearTextToNextEvent = 0.5f;
    [Tooltip("�X�g�b�N�����炵�Ă���Q�[�������X�^�[�g����܂ł̎���"), SerializeField] float waitTimeToRestart = 1;
    [Tooltip("�X�g�b�NUI"), SerializeField] GameObject stockUISet;
    [Tooltip("�X�g�b�N�e�L�X�g"), SerializeField] TextMeshProUGUI stockText;
    [Tooltip("�t�F�C�h�C��������w�i"), SerializeField] FadeInImage blackBackGround;

    private void Awake()
    {
        playerStatus = FindObjectOfType<PlayerStatus>();
        //�ŏ��͔�\��
        gameOverText.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        //�X�g�b�N��stockText�ɕ\��������B
        playerStatus.Stock.Subscribe(stock => stockText.text = stock.ToString()).AddTo(this);

        StartCoroutine(DieSceneCoroutine());
    }

    /// <summary>
    /// diescene�̃R���[�`��
    /// </summary>
    /// <returns></returns>
    IEnumerator DieSceneCoroutine()
    {
        var appearDieTextWait = new WaitForSeconds(waitTimeFromDieToAppearText);
        var fadeDieTextWait = new WaitForSeconds(waitTimeUntilDieTextFade);
        var stockTextAppearWait = new WaitForSeconds(waitTimeFromDieTextToStockText);
        var stockRemoveWait = new WaitForSeconds(waitTimeFromStockTextToRemoveStock);
        var smallWait = new WaitForSeconds(waitTimeFromAppearTextToNextEvent);
        var restartWait = new WaitForSeconds(waitTimeToRestart);

        AudioManager.Instance?.StopAudio();
        AudioManager.Instance?.PlaySE(AudioClipData.SeAudioClipName.Die);
        yield return appearDieTextWait; //�e�L�X�g�\����ҋ@
        gameOverText.SetActive(true); //�e�L�X�g�\��
        AudioManager.Instance?.PlayAudio(AudioClipData.AudioClipName.DieBGM); //���Sbgm�Đ�
        yield return smallWait; //�����ҋ@
        yield return blackBackGround.FadeInCoroutine(); //�u���b�N�A�E�g
        yield return fadeDieTextWait; //�e�L�X�g��\����ҋ@
        gameOverText.SetActive(false); //�e�L�X�g��\��
        yield return stockTextAppearWait; //�X�g�b�NUI�\���ҋ@
        stockUISet.SetActive(true); //�X�g�b�NUI
        yield return stockRemoveWait; //�X�g�b�N�����炷�܂őҋ@
        playerStatus.ReduceStock();�@//�X�g�b�N�����炷
        yield return restartWait; //���X�^�[�g�ҋ@
        GameManager.Instance?.RestartGame(); //���X�^�[�g

    }

   

    
}
