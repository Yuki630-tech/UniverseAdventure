using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class GoalScene : MonoBehaviour
{
    [Tooltip("�^�C�g���V�[���̃{�^��"), SerializeField] Button titleButton;
    [Tooltip("fadein������w�i�摜"), SerializeField] FadeInImage fadeInImage;
    [Tooltip("�N���A�V�[���ŕ\��������e�L�X�g�ƃ{�^���̃Z�b�g"), SerializeField] GameObject goalTextAndButton;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        titleButton.OnClickAsObservable().Subscribe(_ => SceneManager.LoadScene("TitleScene")).AddTo(this);
        //�ŏ���UI��\��
        goalTextAndButton.SetActive(false);
        fadeInImage.gameObject.SetActive(false);
        StartCoroutine(GoalSceneGoroutine());
    }

    /// <summary>
    /// �S�[���V�[����\��������R���[�`��
    /// </summary>
    /// <returns></returns>
    IEnumerator GoalSceneGoroutine()
    {
        
        fadeInImage.gameObject.SetActive(true);
        yield return fadeInImage.FadeInCoroutine();
        goalTextAndButton.SetActive(true);
        GameManager.Instance?.OnGoalSceneCompleted();
    }
}
