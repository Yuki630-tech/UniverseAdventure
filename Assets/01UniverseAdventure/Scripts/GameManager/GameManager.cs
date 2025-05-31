using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using System;
using Cysharp.Threading.Tasks;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    [Tooltip("�v���C���[�����S�������ɕ\��������UI�̃v���n�u"), SerializeField] GameObject playerDieCanvasPrefab;
    [Tooltip("�S�[�������ۂɕ\��������UI�̃v���n�u"), SerializeField] GameObject goalCanvasPrefab;
    [Tooltip("�Q�[���I�[�o�[�����ۂɕ\��������UI�̃v���n�u"), SerializeField] GameObject gameOverCanvasPrefab;

    [Header("�V�[����ɂ���v���C���["), SerializeField] Player player;
    [Header("���S������Ƀv���C���[�����X�|�[������ꏊ�̍��W"), SerializeField] Vector3 restartPos;
    [Header("���S�����ۂɕ\�����ꂽUI"), SerializeField] GameObject gameOverCanvas;
    [Header("�S�[���������ɕ\�����ꂽUI"), SerializeField] GameObject goalCanvas;

    //�T�u�W�F�N�g�̃Z�b�g
    Subject<Unit> onPlayerDieSubject = new Subject<Unit>();
    Subject<Unit> onPlayerRestartSubject = new Subject<Unit>();
    Subject<Unit> onGameClearOrOverSubject = new Subject<Unit>();
    Subject<Unit> onResetGameSubject = new Subject<Unit>();
    Subject<Unit> onPauseGameSubject = new Subject<Unit>();
    Subject<Unit> onUnPauseGameSubject = new Subject<Unit>();


    /// <summary>
    /// �v���C���[�����񂾂Ƃ��̏�����o�^���邽�߂�IObservable
    /// </summary>
    public IObservable<Unit> OnPlayerDieObservable => onPlayerDieSubject;

    /// <summary>
    /// �Q�[�������X�^�[�g�������̏�����o�^���邽�߂�IObservable
    /// </summary>
    public IObservable<Unit> OnPlayerRestartObservable => onPlayerRestartSubject;

    /// <summary>
    /// �Q�[���N���A��ăv���C�������̏�����o�^���邽�߂�IObservable
    /// </summary>
    public IObservable<Unit> OnResetGameObservable => onResetGameSubject;

    /// <summary>
    /// �Q�[�����N���A�������̏�����o�^���邽�߂�IObservable
    /// </summary>
    public IObservable<Unit> OnGameClearOrOverObservable => onGameClearOrOverSubject;

    /// <summary>
    /// �Q�[�����|�[�Y�������̏�����o�^���邽�߂�IObservable
    /// </summary>
    public IObservable<Unit> OnPauseGameObservable => onPauseGameSubject;

    /// <summary>
    /// �Q�[�����A���|�[�Y�������̏�����o�^���邽�߂�IObservable
    /// </summary>
    public IObservable<Unit> OnUnPauseGameObservable => onUnPauseGameSubject;
    
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// �����v���C���ǂ���(�����v���C�ȊO�̍ۂɂ̓^�C�g����ʂ̉��o���X�L�b�v�ł���悤�ɂ���)
    /// </summary>
    public bool IsFirstPlay { get; private set; }

    /// <summary>
    /// �Q�[�����|�[�Y���ꂽ��Ԃ��ǂ���
    /// </summary>
    public bool IsPausing {  get; private set; }

    private void Awake()
    {
        //�V���O���g��
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }

        //subject���Q�[���I�u�W�F�N�g�̔j���ƂƂ��ɔj�������悤�ɂ���
        onPlayerDieSubject.AddTo(this);
        onPlayerRestartSubject.AddTo(this);
        onGameClearOrOverSubject.AddTo(this);
        onPauseGameSubject.AddTo(this);
        onUnPauseGameSubject.AddTo(this);
        IsFirstPlay = true;
        IsPausing = false;

    }

    /// <summary>
    /// �Q�[�����X�^�[�g������֐�
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
        IsFirstPlay = false;
    }

    /// <summary>
    /// �Q�[������߂�֐�
    /// </summary>
    public void QuitGame()
    {
#if�@UNITY_EDITOR
        EditorApplication.isPlaying = false;

#elif PLATFORM_WEBGL
        SceneManager.LoadScene("TitleScene");

#else
        Application.Quit();
#endif
    }



    /// <summary>
    /// gameOver�������ƂɃQ�[�����Z�b�g��ʒm����֐�(�v���C���[�̃X�g�b�N�������l�ɖ߂������Ȃ�)
    /// </summary>
    public void ResetGame()
    {
        onResetGameSubject.OnNext(Unit.Default);
    }

    /// <summary>
    /// �v���C���[��o�^����֐�
    /// </summary>
    /// <param name="setPlayer"></param>
    public void SetPlayer(Player setPlayer)
    {
        player = setPlayer;
    }

    /// <summary>
    /// ���X�|�[�������̍��W��o�^����֐�
    /// </summary>
    /// <param name="setPos"></param>

    public void SetRestartPos(Vector3 setPos)
    {
        restartPos = setPos;
    }

    /// <summary>
    /// �Q�[���̃��X�^�[�g����
    /// </summary>

    public void RestartGame()
    {
        //�v���C���[���`�F�b�N�|�C���g�ɖ߂���hp�������l�ɖ߂�����������
        player.transform.position = restartPos;
        player.OnRestart();
        if(player.transform.position == restartPos)
        {
            player.ChangeToMoveStateTask();
        }

        //���X�^�[�g�������Ƃ�ʒm����
        onPlayerRestartSubject.OnNext(Unit.Default);
        Destroy(gameOverCanvas);
        
    }

    /// <summary>
    /// �v���C���[�����S�������̏���
    /// </summary>
    public void OnPlayerDie()
    {
        //�v���C���[�����񂾂��Ƃ�ʒm����
        onPlayerDieSubject.OnNext(Unit.Default);

        //���S�V�[���̕\��
        gameOverCanvas = Instantiate(playerDieCanvasPrefab);
    }

    /// <summary>
    /// �v���C���[���S�[���������̏���
    /// </summary>

    public void Goal()
    {
        //�S�[���V�[���\��
        goalCanvas = Instantiate(goalCanvasPrefab);
        //�Q�[���N���A�������Ƃ�ʒm����
        onGameClearOrOverSubject.OnNext(Unit.Default);
    }

    /// <summary>
    /// �S�[���V�[���̈�A�̗��ꂪ�I���������̏���
    /// </summary>
    public void OnGoalSceneCompleted()
    {
        //�v���C���[���\���Ɂ��v���C���[�ɂ����鏔�X�̏������I�������邽��
        player.gameObject.SetActive(false);
    }

    /// <summary>
    /// �Q�[���I�[�o�[����
    /// </summary>

    public void OnGameOver()
    {
        DebugLog.Log("�Q�[���I�[�o�[�ł�");
        //�Q�[���I�[�o�[�V�[����\��
        gameOverCanvas = Instantiate(gameOverCanvasPrefab);
        //�Q�[���I�[�o�[�������Ƃ�ʒm����
        onGameClearOrOverSubject.OnNext(Unit.Default);
    }
    /// <summary>
    /// �Q�[�����|�[�Y����
    /// </summary>
    public void OnPauseGame()
    {
        //�Q�[�����|�[�Y���ꂽ���Ƃ�ʒm����
        onPauseGameSubject.OnNext(Unit.Default);

        //�|�[�Y���[�h��
        IsPausing = true;
    }

    /// <summary>
    /// �Q�[�����A���|�[�Y����
    /// </summary>
    public void OnUnPauseGame()
    {
        //�Q�[�����A���|�[�Y���ꂽ���Ƃ�ʒm����
        onUnPauseGameSubject.OnNext(Unit.Default);

        //�|�[�Y���[�h����
        IsPausing = false;
    }
}
