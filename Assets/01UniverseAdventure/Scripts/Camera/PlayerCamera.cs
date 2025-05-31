using Cinemachine;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using DG;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour
{
    [Tooltip("�f���̐��������ւ̉�]�X�s�[�h"), SerializeField] float rotateSpeed;
    [Tooltip("�X�^�[�����O�ɏ���Ă���Ƃ��̃v���C���[�̏�����ւ̉�]�X�s�[�h"), SerializeField] float rotateSpeedWhileFlying;
    [Tooltip("�f���̐������������m���邽�߂ɓo�^����v���C���[��Gravity"), SerializeField] Gravity playerGravity;
    [Tooltip("�v���C���[���X�^�[�����O�ɏ������Ԃ��ǂ���"), SerializeField] bool isFlying;
    [Tooltip("�v���C���[�p��VirtualCamera�B���f����ŏ�Q���ŎՂ�ꂽ����pov�̒l��ς��邽�߂ɕێ�"), SerializeField] CinemachineVirtualCamera playerVirtualCamera;
    [Tooltip("��Q���Ƃ݂Ȃ����C���["), SerializeField] LayerMask layerMask;
    [Tooltip("�����Ƃ݂Ȃ�����"), SerializeField] float verticalDot = 0f;
    [Tooltip("���s�Ƃ݂Ȃ�����"), SerializeField] float parallelDot = 0.98f;
    [Tooltip("�v���C���[�����Ɍ���������pov�̐������̒l"), SerializeField] float downPovVerticalValue = -45f;
    [Tooltip("�v���C���[����Ɍ���������pov�̐������̒l"), SerializeField] float upPovVerticalValue = 45f;
    [Tooltip("pov�l�𑀍삷��DOTween��duration"), SerializeField] float durationOfPovChange = 1f;

    [Header("���[���h��Ԃ̏������Gravity��NormalVec�Ƃ�Dot"), SerializeField] ReactiveProperty<float> dotBetweenUpAndNormalVec = new ReactiveProperty<float>();

    /// <summary>
    /// �v���C���[�����f����ɂ��邩�ǂ���
    /// </summary>
    bool isOnSmallPlanet;
    [SerializeField]
    ReactiveProperty<bool> isNotOnNormalPlanet = new ReactiveProperty<bool>();

    CinemachinePOV pov;
    enum PlayerPos
    {
        up,
        down
    }

    PlayerPos playerPos;

    /// <summary>
    /// �����̉�]�l
    /// </summary>
    Quaternion startRot;

    private void Awake()
    {
        playerPos = PlayerPos.up;
        //������]���擾
        startRot = transform.rotation;

        //�J�����}�l�[�W���[�Ɏ��g��PlayerCamera�Ƃ��ēo�^���J�����}�l�[�W���[����InitializeRot()���Ăяo����悤�ɂ��A
        //���X�N���[�����[�h�ɂȂ������ɉ�]�l�������l�ɖ߂���悤�ɂ��邽�߁B
        CameraManager.Instance?.SetPlayerCamera(this);

        //�v���C���[�����f���̑��ʂɗ���������POV��ω�������悤�ɂ���
        dotBetweenUpAndNormalVec.Where(dotBetweenUpAndNormalVec => dotBetweenUpAndNormalVec == verticalDot && isOnSmallPlanet)�@//���[���h��Ԃ̏�����ɑ΂���Gravity��NormalVec�������ɂȂ��Ă���ꍇ
                                                                                                                               //���f���̑��ʂɂ�����
            .Subscribe(_ => ChangePOVOnSmallPlanet()).AddTo(gameObject);
        
        dotBetweenUpAndNormalVec.Where(dotBetweenUpAndNormalVec => dotBetweenUpAndNormalVec >= parallelDot && isOnSmallPlanet).Subscribe(_ =>
        {
            playerPos = PlayerPos.up;

        }).AddTo(gameObject);
        dotBetweenUpAndNormalVec.Where(dotBetweenUpAndNormalVec => dotBetweenUpAndNormalVec <= -parallelDot && isOnSmallPlanet).Subscribe(_ => playerPos = PlayerPos.down).AddTo(gameObject);
        isNotOnNormalPlanet.Where(isNotOnNormalPlanet => isNotOnNormalPlanet).Subscribe(_ =>
        {
            var bodyUp = transform.up;
            var up = Vector3.up;
            transform.rotation = Quaternion.FromToRotation(bodyUp, up) * transform.rotation;
            
        });

    }
    private void Update()
    {
        isOnSmallPlanet = playerGravity.Planet != null && playerGravity.Planet.IsSmall;
        isNotOnNormalPlanet.Value = (playerGravity.PlanetObj == null || isOnSmallPlanet) && !isFlying;
        //�X�^�[�����O�ɏ���Ă���Ƃ��ɂ̓v���C���[�̏�����A�f����ɂ���Ƃ���gravity.NormalVec�����ɃJ��������]������
        if (isFlying)
        {
            SetCameraRotWhileFlying();
        }

        else
        {
            SetCameraRot();
        }

    }
    /// <summary>
    /// ��]�l�������l�ɖ߂��֐�
    /// </summary>

    public void InitializeRot()
    {
        transform.rotation = startRot;
    }
    #region Planet
    /// <summary>
    /// �J�����̏�������f���̐��������Ɍ����悤�ɉ�]������֐��B�f����ɂ��Ȃ��Ƃ��ɂ̓��[���h���W�̏�����ɃJ�����̏��������������
    /// </summary>

    void SetCameraRot()
    {
        var bodyUp = transform.up;
        //�f����ɂ���ꍇ�f����NormalVec�ɃJ�����̏�����������悤�ɂ���
        if (playerGravity.PlanetObj != null && !isOnSmallPlanet)
        {
            var playerUp = playerGravity.NormalVec;

            var rotation = Quaternion.FromToRotation(bodyUp, playerUp) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed * Time.deltaTime);
        }

        //���f����ɂ���ꍇNormalVec��Vector3.up�Ƃ̓��ς��擾���遨POV�̐������̒l�����̓��ςɉ����ĕω�������
        //�f���̗����������₷�����邽��
        else if (isOnSmallPlanet)
        {
            dotBetweenUpAndNormalVec.Value = Vector3.Dot(Vector3.up, playerGravity.NormalVec);
        }

    }
    #endregion

    #region StarRing
    /// <summary>
    /// �X�^�[�����O�ɏ���Ă���Ƃ��ɃJ�����̏�������v���C���[�̏�����������悤�ɉ�]������֐��B
    /// </summary>
    void SetCameraRotWhileFlying()
    {
        var bodyUp = transform.up;
        var playerUp = playerGravity.transform.up;

        var rotation = Quaternion.FromToRotation(bodyUp, playerUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeedWhileFlying * Time.deltaTime);
    }

    /// <summary>
    /// �v���C���[���X�^�[�����O�ɏ���Ă��邩�ǂ�����o�^����֐�
    /// </summary>
    /// <param name="setFlying"></param>
    public void SetFlying(bool setFlying)
    {
        isFlying = setFlying;
    }
    #endregion

    #region SmallPlanet
    /// <summary>
    /// ���f���ɂ���ԃv���C���[�������Ȃ��Ȃ����ۂ�pov�̐������̒l�𔽓]������֐�
    /// </summary>
    void ChangePOVOnSmallPlanet()
    {
        //LookAt�̃��[�h��POV���[�h����Ȃ���Ώ����𔲂���
        pov = playerVirtualCamera.GetCinemachineComponent<CinemachinePOV>();
        if (pov == null) return;
        switch (playerPos)
        {
            case PlayerPos.up:
                DOTween.To(GetCurrentPovValue, SetCurrentPovValue, downPovVerticalValue, durationOfPovChange);
                playerPos = PlayerPos.down;
                    break;
            case PlayerPos.down:
                DOTween.To(GetCurrentPovValue, SetCurrentPovValue, upPovVerticalValue, durationOfPovChange);
                playerPos = PlayerPos.up;
                break;
        }

    }
    #endregion

    #region Dotween
    /// <summary>
    /// DOTween�̃A�j���[�V���������邽�߂ɏ����l���擾����֐�
    /// </summary>
    /// <returns></returns>
    float GetCurrentPovValue() => pov.m_VerticalAxis.Value;

    void SetCurrentPovValue(float value)
    {
        pov.m_VerticalAxis.Value = value;
    }
    #endregion
}

