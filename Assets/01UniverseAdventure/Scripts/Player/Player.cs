using Cinemachine;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Splines;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerMover playerMover;
    [SerializeField] PlayerFlyToNextStage playerFlyToNextStage;
    [SerializeField] PlayerDrawnIntoBlackHole playerDrawnIntoBlackHole;
    [SerializeField] PlayerDamage playerDamage;
    [SerializeField] PlayerStatus playerStatus;
    [SerializeField] State currentState;
    [Tooltip("�u���b�N�z�[���ɋz�����܂��Ƃ��ɃJ�������ǂꂾ�����ɉ����邩"), SerializeField] float cameraDistance = 5f;
    [SerializeField] Rigidbody rb;
    [SerializeField] float cameraRotateSpeedWhenBlackHole = 1;
    [SerializeField] Animator animator;
    [SerializeField] PlayerCamera playerCamera;
    [SerializeField] Camera mainCamera;

    /// <summary>
    /// State�̖��O
    /// </summary>
    public enum State
    {
        Move,
        FlyToNextStage,
        BlackHole,
        Damage,
        Die
    }

    private void Awake()
    {
        //���g��GameManager�ɓo�^���Q�[�������X�^�[�g�����ۂ�GameManager���v���C���[���`�F�b�N�|�C���g�܂ňړ������邱�Ƃ��ł���悤�ɂ���
        GameManager.Instance?.SetPlayer(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeToMoveState();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Move:
                playerMover.UpdateMove();
                break;
            case State.FlyToNextStage:
                playerFlyToNextStage.UpdateFly();
                break;
            case State.BlackHole:
                playerDrawnIntoBlackHole.UpdateGetDrawn();
                break;
            case State.Damage:
                //�_���[�W��Ԃł͂Ȃ��Ȃ����Ƃ��ɁA�X�^�[�����O�ɏ������ԂŒʏ�ړ��̏�ԂɂȂ�Ȃ��悤��
                //FlyToNext�X�e�[�g�̏ꍇ�ȊO�̎���MoveState�Ɉڍs����悤�ɂ���B
                if (!playerDamage.IsDamaged && currentState != State.FlyToNextStage)
                {
                    ChangeToMoveState();
                    return;
                }
                break;
            case State.Die:

                break;
        }
    }
    
    /// <summary>
    /// MoveState�Ɉڍs������֐��B
    /// </summary>
    public void ChangeToMoveState()
    {
        //���S�������A�u���b�N�z�[���ɋz�����܂�Ă���Œ���MoveState�Ɉڍs���Ȃ��悤�ɂ���
        if (currentState == State.Die || currentState == State.BlackHole) return;
        ChangeToMoveStateTask();
        
    }

    public void ChangeToMoveStateTask()
    {
        if (playerMover != null)
        {
            playerMover.GoToMovable();
        }

        currentState = State.Move;
        rb.isKinematic = false;
        playerFlyToNextStage.SplineAnimate.enabled = false;
        //FlyToNext�X�e�[�g�ɕς�钼�O�ɋL�������x�N�g�����K�p����ăv���C���[������Ɉړ����邱�Ƃ̂Ȃ��悤��
        //�ړ��x�N�g�������Z�b�g����
        playerMover.InitializeVector();

        //�J�������X�^�[�����O�p�̃J��������ʏ�̃J�����ɐ؂�ւ���
        playerCamera.SetFlying(false);
    }

    public void ChangeToFlyState(SplineContainer spline)
    {
        if (playerMover != null)
        {
            playerMover.GoToImmovable();
        }
        //�������Z���~
        rb.isKinematic = true;
        //�X�v���C���R���|�[�l���g�L����
        playerFlyToNextStage.StartFlyToNextStage(spline);
        currentState = State.FlyToNextStage;
        //�J�������X�^�[�����O�p�̃J�����ɐ؂�ւ���B
        playerCamera.SetFlying(true);
    }

    public void ChangeToBlackHoleState(Transform setTransform)
    {
        //�u���b�N�z�[���ɋz�����܂�Ă���Œ��A���S���A�X�^�[�����O�ɏ���Ă���ԂɃu���b�N�z�[���ɋz�����܂�邱�Ƃ�h��
        if (currentState == State.BlackHole || currentState == State.FlyToNextStage || currentState == State.Die) return;
        rb.isKinematic = true;
        //�z�����܂��u���b�N�z�[����o�^
        playerDrawnIntoBlackHole.StartGetDrawn(setTransform);
        currentState = State.BlackHole;
        //----------------------�J�������[�N----------------------------------------------
        playerMover.MainCamera.GetComponent<CinemachineBrain>().enabled = false;
        //�v���C���[����]�̒��S����u���b�N�z�[���̕����������悤�ɉ�]�l��ݒ肵���̕����Ɍ�������
        var direction = setTransform.position - playerDrawnIntoBlackHole.Center;
        direction = direction.normalized;
        var look = Quaternion.LookRotation(direction);
        StartCoroutine(CameraManager.Instance?.RotateCameraTo(look, cameraRotateSpeedWhenBlackHole));

        //���S����cameraDistance�������ɉ��������ʒu�ɃJ�������ړ����~�^�������₷�����邽��
        var position = playerDrawnIntoBlackHole.Center - playerDrawnIntoBlackHole.DirectionFromCenterToBlackHole * cameraDistance;
        StartCoroutine(CameraManager.Instance?.MoveCameraTo(position, cameraDistance));
    }

    /// <summary>
    /// �_���[�W���󂯂�֐���AttackReceiver�̃C�x���g�Ƃ��ēo�^
    /// </summary>
    /// <param name="data"></param>
    public void OnDamage(AttackData data)
    {
        //���G��ԁA���S��ԁA�u���b�N�z�[���ɋz�����܂�Ă���Œ��A�X�^�[�����O�ɏ���Ă���ԁA�Q�[�����|�[�Y���Ă���ԂɃ_���[�W���󂯂Ȃ��悤�ɂ���B
        bool isDisable = playerDamage.IsInvincible || currentState == State.Die || currentState == State.BlackHole || currentState == State.FlyToNextStage;
        if (isDisable || (GameManager.Instance != null &&  GameManager.Instance.IsPausing))
        {
            return;
        }
        currentState = State.Damage;
        playerDamage.OnTakeDamage(data);
        if (playerDamage.PlayerStatus.PlayerStatusData.Hp == 0)
        {
            OnDie();
        }
    }

    /// <summary>
    /// �v���C���[�̎��S�������s���֐��B
    /// </summary>
    public void OnDie()
    {
        //���S��Ԃł���Ɏ��S���Ă��܂����Ƃ�h��
        if(currentState == State.Die) { return; }   
        animator.SetTrigger("Die");
        playerDamage.StopFlash();
        //�u���b�N�z�[���ɋz�����܂�Ă���Ԃ�Die�X�e�[�g�ɂ���Ɖ�]�����f���������Ă��܂��̂Ńu���b�N�z�[���ɋz�����܂�Ă���Ԃ�Die�X�e�[�g�ɂȂ�Ȃ��悤�ɂ���
        if(currentState != State.BlackHole)
        {
            currentState = State.Die;
        }
        if(playerStatus.Stock.Value > 0)
        {
            GameManager.Instance?.OnPlayerDie();

        }

        else
        {
            GameManager.Instance?.OnGameOver();
        }

    }

    /// <summary>
    /// �Q�[�������X�^�[�g�����ۂ̃v���C���[�̏���
    /// </summary>
    public void OnRestart()
    {
        playerStatus.RestartStatus();
        animator.ResetTrigger("Die");
        animator.SetTrigger("Restart");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            GameManager.Instance?.Goal();
        }
    }

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }
}
