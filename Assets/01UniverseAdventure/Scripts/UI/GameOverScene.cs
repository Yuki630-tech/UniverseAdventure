using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScene : MonoBehaviour
{
    [Tooltip("�Q�[���I�[�o�[���ɕ\��������e�L�X�g�ƃ{�^���̃Z�b�g"), SerializeField] GameObject gameOverTextAndButton;
    [Tooltip("�Q�[���I�[�o�[�{�^��"), SerializeField] Button gameOverButton;
    [Tooltip("fadein������w�i"), SerializeField] FadeInImage fadeInImage;
    [Tooltip("���S���Ă��玀�S�W���O����炷�܂ł̎���"), SerializeField] float intervalUntilAudioPlay;
    private void OnEnable()
    {
        //�ŏ��͂��ׂĂ�UI��\��
        gameOverTextAndButton.SetActive(false);
        fadeInImage.gameObject.SetActive(false);
        //�{�^������������TitleScene�ɖ߂�悤�ɂ���B�܂��Q�[�������Z�b�g
        gameOverButton.OnClickAsObservable().Subscribe(_ =>
        {
            SceneManager.LoadScene("TitleScene");
            GameManager.Instance?.ResetGame();
        }).AddTo(gameObject);
        StartCoroutine(GameOverCoroutine());
    }

    /// <summary>
    /// �Q�[���I�[�o�[�V�[���̃R���[�`���B���ʉ����ҋ@�����Sbgm���w�ifadein���e�L�X�g�A�{�^���\��
    /// </summary>
    /// <returns></returns>
    IEnumerator GameOverCoroutine()
    {
        if (AudioManager.Instance == null) yield break;
        var waitForPlayAudio = new WaitForSeconds(intervalUntilAudioPlay);
        var waitWhileAudioPlaying = new WaitWhile(() => AudioManager.Instance.IsAudioPlaying());
        AudioManager.Instance.StopAudio(); //�����̏�Ԃɂ���
        AudioManager.Instance.PlaySE(AudioClipData.SeAudioClipName.Die); //���S��SE
        yield return waitForPlayAudio;
        AudioManager.Instance.PlayAudio(AudioClipData.AudioClipName.GameOverBGM);
        yield return fadeInImage.FadeInCoroutine();
        yield return waitWhileAudioPlaying;
        gameOverTextAndButton.SetActive(true);
        AudioManager.Instance.PlayAudio(AudioClipData.AudioClipName.TitleBGM);


    }
}
