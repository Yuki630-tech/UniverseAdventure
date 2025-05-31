using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Cinemachine.Utility;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;
using Cinemachine;

public class PlayerMover : Movable
{
    [Tooltip("���͂𐧌䂷��}�l�[�W���["), SerializeField] InputManager inputManager;
    [Tooltip("������炷�I�[�f�B�I�\�[�X"), SerializeField] AudioSource audioSource;
    [Tooltip("�����̃I�[�f�B�I�N���b�v"), SerializeField] AudioClip footAudioClip;
    [Tooltip("�W�����v���̃I�[�f�B�I�N���b�v"), SerializeField] AudioClip jumpAudioclip;
    [Tooltip("�ړ����x"), SerializeField] float moveSpeed;
    [Tooltip("�ڒn������s���R���|�[�l���g"), SerializeField] GroundChecker groundChecker;
    [Tooltip("�W�����v��"), SerializeField] float jumpPower;
    [Tooltip("�v���C���[�̃A�j���[�^�[�R���|�[�l���g"), SerializeField] Animator animator;

    [Tooltip("�ړ���i�Ƃ��ėp���镨������"), SerializeField] Rigidbody rb;
    [Tooltip("�v���C���[�̏d�͂𑀍삷��R���|�[�l���g"), SerializeField] Gravity gravity;
    [Tooltip("�V�[����̃��C���J����"), SerializeField] Camera mainCamera;
    [Tooltip("�ڒn��Ԃ��ǂ���"), SerializeField] bool isGround;
    [Tooltip("�J�����ƃv���C���[�̏�����Ƃ𐂒��Ƃ݂Ȃ�����"), SerializeField] float verticalDot = 0f;
    [Tooltip("�v���C���[���f��virtualcamera"), SerializeField] CinemachineVirtualCamera playerVirtualCamera;
    CinemachinePOV pov;
    Vector3 moveDirection;
    bool isMovable;
    Vector3 basedVector;
    Vector3 right;
    Vector3 forward;
    [Tooltip("�v���C���[�̏�ɃJ���������邩�ǂ���"), SerializeField] bool isUp;
    bool isUpSideDown;
    [Tooltip("���s��ԂƂ݂Ȃ����ς̍ő�l"), SerializeField] float parallelThreshould = 0.8f;
    [Tooltip("�㉺�������܂ł���Ƃ݂Ȃ����ς̍ő�l"), SerializeField] float upsideDownThreshould = 0.4f;
    bool planetIsSmallBox;
    bool isOnlyMoveToSide;
    bool isOnSideOfSmallPlanet;

    [SerializeField] ReactiveProperty<float> cameraRotProperty = new ReactiveProperty<float>();
    public override bool IsGround { get => isGround; }
    public Camera MainCamera { get => mainCamera; }
    public bool IsOnlyMoveToSide { get => isOnlyMoveToSide; }

    // Start is called before the first frame update
    void Start()
    {
        pov = playerVirtualCamera.GetCinemachineComponent<CinemachinePOV>();
        isMovable = true;
        //�L�[�{�[�h�A�R���g���[���[�̑���̓��͂��ω������ۂɓ���������ݒ肷��B
        inputManager.DirectionInput.Where(_ => isMovable && !isOnlyMoveToSide).Subscribe(input => GetAdjustedVector(input)).AddTo(this);

        //���X�N���[��
        inputManager.DirectionInput.Where(_ => isMovable && isOnlyMoveToSide).Subscribe(input => GetAdjustedVector(new Vector3(input.x, 0f, 0f))).AddTo(this);

        //�J�����ɍ��킹�ĉ�]
        cameraRotProperty.Where(_ => isMovable && !isOnlyMoveToSide)
            .Where(_ => gravity.Planet == null || !gravity.Planet.IsSmall || isOnSideOfSmallPlanet)
            .Subscribe(cameraRot =>
        {
            GetAdjustedVector(inputManager.DirectionInput.Value);
        });

        //�Q�[�����|�[�Y�������Ƀv���C���[�������Ȃ��悤�ɂ���
        GameManager.Instance?.OnPauseGameObservable.Subscribe(_ => GoToImmovable()).AddTo(gameObject);

        //�Q�[�����|�[�Y���烊�X�^�[�g�������Ƀv���C���[��������悤�ɂ���
        GameManager.Instance?.OnUnPauseGameObservable.Subscribe(_ => GoToMovable()).AddTo(gameObject);

        //�v���C���[������ŃQ�[�������X�^�[�g�������ɉ��X�N���[�����[�h����������
        GameManager.Instance.OnPlayerRestartObservable.Subscribe(_ => isOnlyMoveToSide = false);

    }

    /// <summary>
    /// ���X�N���[���ƒʏ�ړ��Ƃ�؂�ւ���֐�
    /// </summary>
    /// <param name="setModeBool"></param>
    public void SetIfOnlyMoveToSide(bool setModeBool)
    {
        isOnlyMoveToSide = setModeBool;
    }
    /// <summary>
    /// Move�̍X�V�֐�
    /// </summary>

    // Update is called once per frame
    public void UpdateMove()
    {
        cameraRotProperty.Value = pov.m_HorizontalAxis.Value;
        isOnSideOfSmallPlanet = gravity.Planet != null && gravity.Planet.IsSmall && Vector3.Dot(transform.up, Vector3.up) != verticalDot;
        //�ڒn��Ԃ̎�
        if (groundChecker.IsGround)
        {
            //�W�����v�{�^���������ꂽ�Ƃ��Q�[�����|�[�Y���Ă��Ȃ��ăv���C���[���������ԂȂ�W�����v
            if (inputManager.JumpInput.Value && !GameManager.Instance.IsPausing && isMovable)
            {
                var jumpTask = Jump();
            }
        }
       
        groundChecker.UpdateGroundCheck();

        //�������Ԃ�moveDirection�̑傫����0�ł͂Ȃ��ꍇ�v���C���[��O�����Ɉړ�������
        if (isMovable)
        {
            if (moveDirection.magnitude > 0)
            {
                

                rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.deltaTime); //���͂ɂ������������ɐi��

            }
        }

        else
        {
            moveDirection = Vector3.zero;
        }


        if (isGround)
        {
            animator.SetFloat("Speed", moveDirection.magnitude);
        }

        else
        {
            animator.SetFloat("Speed", 0f);

        }
        isGround = groundChecker.IsGround;
        animator.SetBool("isGrounded", isGround);

    }

    /// <summary>
    /// �X�^�[�����O�ňړ��㒅�n�������ɃX�^�[�����O�ɏ��O�̓��̓x�N�g�����󂯎�����܂܏���ɕ����n�߂�o�O��h�����߁A���n�����ۂɓ��̓x�N�g�������Z�b�g����
    /// </summary>
    public void InitializeVector()
    {
        GetAdjustedVector(Vector3.zero);
    }

    /// <summary>
    /// Player��i�s�����Ɍ�����
    /// </summary>
    /// <param name="input"></param>
    void GetAdjustedVector(Vector3 input)
    {
        DecideAxis();
        //�n�ʂ̐�����basedVector�Ƃ̊O�ς��ړ����̉E�����Ƃ���
        right = Vector3.Cross(gravity.NormalVec, basedVector).normalized;
        //�E�����ƒn�ʂ̐����Ƃ̊O�ς��ړ����̑O�����Ƃ���
        forward = Vector3.Cross(right, gravity.NormalVec).normalized;

        //�v���C���[�ɂ�����d�͂�������ŁA�f����ɂ��Ȃ��ꍇ�A�E�������t�ɂ���
        right = isUpSideDown && !isUp ? -right : right;
        moveDirection = right * input.x + forward * input.y;
        moveDirection = moveDirection.normalized;

        if (moveDirection.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection, transform.up); //���g�̏���������Ƃ���moveDirection�̂ق��Ƀv���C���[��������
        }

       

    }

    /// <summary>
    /// gravity.NormalVec�ɑ΂��Ăǂ̃x�N�g���Ƃ̊O�ς�i�s�x�N�g���ɂ�����E�����ƒ�`���邩�����߂�
    /// ��{�I�ɃJ�����̌����Ă�������B�J�������^��ɂ��遨�J�����̏�����A�v���C���[���㉺�������܂ɂȂ��Ă��遨�J�����̌�����
    /// </summary>
    /// <returns></returns>
    void DecideAxis()
    {

        planetIsSmallBox = gravity.PlanetObj != null && gravity.Planet.IsSmall;
        //�n�ʂ̐����ƃJ�����̌����Ă�������Ƃ��قڕ��s(�J�������v���C���[�̐^��ɂ���)�̏ꍇ��
        //gravity.NormalVec�ƃJ�����̏�����Ƃ̊O�ς�i�s�x�N�g���̉E�����Ƃ��Ē�`����
        if (Vector3.Dot(gravity.NormalVec, mainCamera.transform.forward) < -parallelThreshould)
        {
            isUp = true;
            isUpSideDown = false;
            basedVector = mainCamera.transform.up;

        }
        //�n�ʂ̐����ƃJ�����̏�������قڐ����΂̕����Řf����Ƀv���C���[������Ƃ�(�v���C���[���㉺�������܂ɉf���Ă���Ƃ�)
        //gravity.NormalVec�ƃJ�����̌������Ƃ̊O�ς�i�s�x�N�g���̉E�����Ƃ��Ē�`����
        else if (Vector3.Dot(gravity.NormalVec, mainCamera.transform.up) < -upsideDownThreshould && planetIsSmallBox)
        {
            isUp = false;
            isUpSideDown = false;
            basedVector = -mainCamera.transform.forward;
        }
        //��L�ȊO�̏ꍇ��gravity.NormalVec�ƃJ�����̑O�����Ƃ̊O�ς�i�s�x�N�g���̉E�����Ƃ��Ē�`����
        else
        {
            isUp = false;
            isUpSideDown = Vector3.Dot(gravity.NormalVec, mainCamera.transform.up) < -upsideDownThreshould; 
            basedVector = mainCamera.transform.forward;

        }
    }

    /// <summary>
    /// �ڒn���Ă���ꍇ�ɃW�����v���͂����邱�ƂŃW�����v������
    /// </summary>

    async UniTask Jump()
    {

        audioSource.PlayOneShot(jumpAudioclip);
        groundChecker.DisableToCheckGround();
        rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
        await UniTask.WaitUntil(() => isGround);
        audioSource.PlayOneShot(footAudioClip);
       
    }

    /// <summary>
    /// ������炷�֐��B���A�j���[�V�����N���b�v�̃C�x���g�Ƃ��ēo�^
    /// </summary>
    void PlayFootSound()
    {
        audioSource.volume = 1;
        audioSource.PlayOneShot(footAudioClip);

    }

    public override void GoToMovable()
    {
        isMovable = true;
        InitializeVector();
    }

    public override void GoToImmovable()
    {
        isMovable = false;
        animator.SetFloat("Speed", 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, moveDirection * 5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, forward * 5f);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, right * 5f);

        //Gizmos.color = Color.green;
        //Gizmos.DrawRay(transform.position, moveDirection * 5f);


    }

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

}