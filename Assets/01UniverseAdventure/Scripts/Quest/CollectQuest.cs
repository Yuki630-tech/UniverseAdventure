using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class CollectQuest : MonoBehaviour
{
    [Tooltip("���̃N�G�X�g�ŏW�߂�A�C�e���ɐݒ肷��^�O�̖��O(�X�e�[�W��(���̃R���|�[�l���g���A�^�b�`����Ă���I�u�W�F�N�g��)"), SerializeField] 
    string collectObjTagName;
    [Tooltip("���̃N�G�X�g���X�^�[�g���邽�߂Ƀv���C���[���G���I�u�W�F�N�g�ɐݒ肷��^�O�̖��O(�X�e�[�W�� + Start)"), SerializeField] string startObjTagName;
    [Tooltip("���̃N�G�X�g�����S�ɏI�����邽�߂ɐG���I�u�W�F�N�g�̖��O"), SerializeField] string quitObjTagName; 
    [Tooltip("�W�߂�ׂ���"), SerializeField] int collectNum;
    [Tooltip("���̃X�e�[�W�ւ̈ړ���i(�N�G�X�g���͔�\���A�������ɕ\��������I�u�W�F�N�g)"), SerializeField] GameObject[] leadToNextStageObjs;
    [Tooltip("�ړ���i���o��������ۂɖ炷SE�̎��"), SerializeField] List<AudioClipData.SeAudioClipName> seAudioClipNames;
    [Tooltip("�\��������I�u�W�F�N�g��2����ۂ�1�O�̃I�u�W�F�N�g��\�������Ă��玟���J�����ŉf���o���܂ł̎��ԁB" +
        "1�̏ꍇ�A�܂��Ō�̃I�u�W�F�N�g�ɂ��Ă�0�ɐݒ肷��"), SerializeField]
    float[] intervals;
    [Tooltip("�v���C���[�̃I�u�W�F�N�g�B�N�G�X�g�J�n�A�I�������m���邽�߂Ɏ擾�B"), SerializeField] GameObject player;

    [Header("���ݏW�߂���"), SerializeField] ReactiveProperty<int> currentCollectNum;
    [Tooltip("�f���o���J�����̖��O(�f�����Ԃɓo�^���邱��)"), SerializeField] string[] cameraName;
    [Tooltip("�v���C���[���f���o���J�����̖��O"), SerializeField] string playerCamera;
    [Tooltip("�J�����ŉf���o���Ă���I�u�W�F�N�g��\��������܂ł̎���"), SerializeField] float activeInterval;
    [Tooltip("�Ō�̃I�u�W�F�N�g���f���Ă���v���C���[���f���܂ł̎���"), SerializeField] float returnCameraPriorityInterval;
    [Tooltip("�v���C���[���f���Ă����ɖ��G��Ԃ���������Ɠ�Փx�������Ȃ�̂ł��΂炭�҂B���̑ҋ@����"), SerializeField] float invincibleInterval = 1f;
    [Tooltip("�W�߂�����\��������UI"), SerializeField] GameObject collectUI;
    [Tooltip("�W�߂�����\��������e�L�X�g"), SerializeField] TextMeshProUGUI collectNumText;

    [Tooltip("�N�G�X�g�����������ۂɕ\��������I�u�W�F�N�g"), SerializeField] ObjectActivatedAfterQuestClear objectActivatedAfterQuestClear;

    IDisposable triggerDisposable;
    List<GameObject> collectedObj = new List<GameObject>(); //�v���C���[�����S���ă��X�^�[�g�������ɏW�߂Ĕ�\���ɂȂ����A�C�e�������Ƃɖ߂����Ƃ��ł���悤�ɁA
                                                            //�W�߂��A�C�e����ێ����Ă������߂̃��X�g
    //�N�G�X�g�Ɋ֌W����subject�Q
    Subject<Unit> onCompleteQuestSubject = new Subject<Unit>();
    Subject<Unit> onStopQuestSubject = new Subject<Unit>();
    Subject<Unit> onReplayQuestSubject = new Subject<Unit>();
    Subject<Unit> onQuitQuestSubject = new Subject<Unit>();

    /// <summary>
    /// �N�G�X�g�������������Ƃ����m����IObservable
    /// </summary>
    public IObservable<Unit> OnCompleteQuestObservable => onCompleteQuestSubject;

    /// <summary>
    /// �N�G�X�g���ꎞ���f�������Ƃ����m����IObservable
    /// </summary>
    public IObservable<Unit> OnStopQuestObservable => onStopQuestSubject;

    /// <summary>
    /// �N�G�X�g�����v���C���ꂽ���Ƃ����m����IObservable
    /// </summary>
    public IObservable<Unit> OnReplayQuestObservable => onReplayQuestSubject;

    /// <summary>
    /// �N�G�X�g�����S�Ɋ����������Ƃ����m����IObservable
    /// </summary>
    public IObservable<Unit> OnQuitQuestObservable => onQuitQuestSubject;
    private void Awake()
    {
        objectActivatedAfterQuestClear?.ChangeActiveState(false);
        //�N�G�X�g���X�^�[�g������I�u�W�F�N�g�Ƀv���C���[���G�ꂽ�Ƃ��N�G�X�g���J�n����悤�ɂ���B
        player.OnTriggerEnterAsObservable().Where(collider => collider.CompareTag(startObjTagName)).Subscribe(_ => StartQuest()).AddTo(this);

        //�v���C���[�����̃X�e�[�W�i�񂾎��ɃN�G�X�g���~�߂�B(�d�͐؂�ւ����X�g�b�v����Ȃ�)
        player.OnTriggerEnterAsObservable().Where(collider => quitObjTagName.Length != 0 && collider.CompareTag(quitObjTagName)).Subscribe(_ => onQuitQuestSubject.OnNext(Unit.Default)).AddTo(this);
        onCompleteQuestSubject.AddTo(this);
        onStopQuestSubject.AddTo(this);
        onReplayQuestSubject.AddTo(this);
        collectUI.SetActive(false);
        GameManager.Instance.OnPlayerRestartObservable.Where(_ => collectedObj.Count > 0).Subscribe(_ =>
        {
            //���X�^�[�g���ɂ��łɎ擾�ς݂̃A�C�e��������΍ĕ\������
            foreach (var obj in collectedObj)
            {
                obj.SetActive(true);
            }

            collectedObj.Clear();
        });

        DeactivateLeadObject();

        GameManager.Instance.OnPlayerRestartObservable.Subscribe(_ =>
        {
            DeactivateLeadObject();
        });
    }

    /// <summary>
    /// �N�G�X�g���X�^�[�g������֐�
    /// </summary>
    public void StartQuest()
    {
        collectUI.SetActive(true);
        //�A�C�e���ɐG�ꂽ�Ƃ��̍w�ǂ�j������
        triggerDisposable?.Dispose();

        //�W�߂���������������
        currentCollectNum.Value = 0;
        

        //�W�߂����ƏW�߂�ׂ�����UI�\��
        currentCollectNum.Subscribe(num => collectNumText.text = num.ToString() + " / " + collectNum.ToString()).AddTo(gameObject);

        //�A�C�e���ɐG�ꂽ�Ƃ��ɃA�C�e�����\���ɂ��Ď��W��������₷�B�܂��A�j���ł���悤��iDisposable�ɍw�ǎ҂�o�^���Ă����B
        triggerDisposable = player.OnTriggerEnterAsObservable().Where(collider => collider.CompareTag(collectObjTagName)).Subscribe(collider => 
        {
            collider.transform.parent.gameObject.SetActive(false);
            collectedObj.Add(collider.transform.parent.gameObject);
            AddCollectNum();
            AudioManager.Instance?.PlaySE(AudioClipData.SeAudioClipName.GetItem);
        }).AddTo(gameObject);

    }

    /// <summary>
    /// ���W���𑝂₷�֐�
    /// </summary>
    public void AddCollectNum()
    {
        currentCollectNum.Value++;
        //���W�����m���}��B��������N�G�X�g�N���A�Ƃ���
        if(currentCollectNum.Value >= collectNum)
        {
            CompleteQuest();
        }
    }

    /// <summary>
    /// �N�G�X�g�N���A�̊֐�
    /// </summary>
    [ContextMenu("PlayEndEvent")]
    
    void CompleteQuest()
    {
        objectActivatedAfterQuestClear?.ChangeActiveState(true);
        collectUI.SetActive(false);
        StartCoroutine(CompleteQuestCoroutine());
        currentCollectNum.Value = 0;

    }

    /// <summary>
    /// ���̃X�e�[�W�ւ̈ړ���i�����ׂĔ�\��
    /// </summary>

    void DeactivateLeadObject()
    {
        foreach (var obj in leadToNextStageObjs)
        {
            obj.SetActive(false);

        }
    }

    /// <summary>
    /// �N�G�X�g�N���A�̃R���[�`��
    /// </summary>
    /// <returns></returns>
    IEnumerator CompleteQuestCoroutine()
    {
        var pauseUI = FindObjectOfType<PauseUI>();
        pauseUI.ChangeIfEnableToPause(false);
        var playerMover = player.GetComponent<PlayerMover>(); //���o���A����Ƀv���C���[�������Ȃ��悤�ɂ��邽�߂�playerMover���擾���Ĉړ��ł��Ȃ�����
        var playerDamage = player.GetComponent<PlayerDamage>(); //���o���A�v���C���[���_���[�W���󂯂Ȃ��悤�ɂ��邽�߂�playeDamage���擾���ă_���[�W���󂯂Ȃ��悤�ɂ���B
        onStopQuestSubject.OnNext(Unit.Default); //�ꎞ��~��ʒm
        playerDamage.ChangeInvicibleValue(true);
        for (int i = 0; i < leadToNextStageObjs.Length; i++)
        {
            int index = i;

            //�J��������
            CameraManager.Instance?.ChangeCamera(cameraName[index]);
            playerMover.GoToImmovable(); //�����Ȃ��悤�ɂ���
            yield return new WaitForSeconds(activeInterval);
            //�ړ���i�\��
            leadToNextStageObjs[i].SetActive(true);
            AudioManager.Instance?.PlaySE(seAudioClipNames[index]);
            yield return new WaitForSeconds(intervals[index]);
        }
        
        yield return new WaitForSeconds(returnCameraPriorityInterval);
        playerMover.GoToMovable(); //������悤�ɂ���
        //�J��������
        CameraManager.Instance?.ChangeCamera(playerCamera);
        pauseUI.ChangeIfEnableToPause(true);
        onReplayQuestSubject.OnNext(Unit.Default); //�N�G�X�g���v���C�̒ʒm
        onCompleteQuestSubject.OnNext(Unit.Default); //�N�G�X�g�N���A�̒ʒm
        yield return new WaitForSeconds(invincibleInterval);
        playerDamage.ChangeInvicibleValue(false); //���G��ԉ���
    }
}
