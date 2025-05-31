using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class CameraManager : MonoBehaviour
{
    [Header("�V�[����ɂ���S�Ă�virtualcamera�̃��X�g")]public List<CinemachineVirtualCamera> cinemachineVirtualCameras = new List<CinemachineVirtualCamera>();
    [Tooltip("�v���C���[�̃J����"),SerializeField] PlayerCamera playerCamera;

    public static CameraManager Instance { get; private set; }
    [Tooltip("�V�[����ɂ��郁�C���J����"), SerializeField] Camera mainCamera;
    [Tooltip("���C���J�����ɃZ�b�g����Ă���CinemachineBrain"), SerializeField] CinemachineBrain mainCameraBrain;
    [Header("�`�F�b�N�|�C���g�ɐG�ꂽ�Ƃ��ɗL���ɂȂ��Ă���J�����̖��O"), SerializeField] string cameraNameBeforeDie;

    private void Awake()
    {
        //�V���O���g��
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }

        //�v���C���[�����X�|�������ۂɃu���b�N�z�[�����o�̂��ߖ��������ꂽmainCameraBrain��L����������
        //�J�������`�F�b�N�|�C���g�ɂ����Ƃ��̂��̂ɐ؂�ւ���
        //pov�̂��ꂼ��̎��̒l��0�ɂ���
        GameManager.Instance?.OnPlayerRestartObservable.Where(_ => !mainCameraBrain.enabled).Subscribe(_ =>
        {
            mainCameraBrain.enabled = true; 
        }
        ).AddTo(this);

        GameManager.Instance?.OnPlayerRestartObservable.Subscribe(_ =>
        {
            var pov = GetCamera("PlayerVirtualCamera").GetCinemachineComponent<CinemachinePOV>();
            if (pov != null)
            {
                pov.m_HorizontalAxis.Value = 0f;
                pov.m_VerticalAxis.Value = 0f;
            }
            ChangeCamera(cameraNameBeforeDie, CinemachineBlendDefinition.Style.Cut, 0f);
        });
        
    }

    /// <summary>
    /// �v���C���[�J������o�^����֐�
    /// </summary>
    /// <param name="setCamera"></param>
    public void SetPlayerCamera(PlayerCamera setCamera)
    {
        playerCamera = setCamera;
    }

    /// <summary>
    /// ���C���J������o�^����֐�
    /// </summary>

    public void SetMainCamera()
    {
        if(mainCamera == null)
        {
            mainCamera = Camera.main;
            mainCameraBrain = mainCamera.GetComponent<CinemachineBrain>();
        }
       
    }

    /// <summary>
    /// �w�肵���J�����ɐ؂�ւ���֐�
    /// </summary>
    /// <param name="cameraName"></param>
    /// <param name="blendStyle"></param>
    /// <param name="blendTime"></param>

    public void ChangeCamera(string cameraName, CinemachineBlendDefinition.Style blendStyle = CinemachineBlendDefinition.Style.EaseInOut, float blendTime = 2.0f)
    {
        //���ݗL���ɂȂ��Ă���J�����Ɛ؂�ւ���J�������V�[���ォ��擾
        var camera1 = cinemachineVirtualCameras.FirstOrDefault(camera => camera.Priority == 10);
        var camera2 = cinemachineVirtualCameras.FirstOrDefault(camera => camera.Name == cameraName);

        //���ꂼ���Priority��ς��邱�ƂŃJ������؂�ւ�
        camera1.Priority = -1;
        camera2.Priority = 10;
        SetBlend(blendStyle, blendTime);

        //�f���ɉ����ăJ�����̐e�I�u�W�F�N�g����]�����Ă���̂ŉ��X�N���[�����[�h�ɂȂ������ɕςȉ�]��ԂɂȂ�Ȃ��悤�ɉ�]��Ԃ�����������
        if (cameraName == "PlayerVirtualCameraFromSide")
        {
            playerCamera.InitializeRot();
        }

    }

    /// <summary>
    /// �u�����h���[�h��ݒ肷��֐�
    /// </summary>
    /// <param name="blendStyle"></param>
    /// <param name="blendTime"></param>
    void SetBlend(CinemachineBlendDefinition.Style blendStyle, float blendTime)
    {
        //�u�����h�^�C�v�ƃ^�C����ݒ肷�邽�߂�CinemachineBlendDefinition���쐬�B���ꂼ��̒l�������Ŏw�肵�����̂ɐݒ�B
        var blend = new CinemachineBlendDefinition();

        blend.m_Style = blendStyle;
        blend.m_Time = blendTime;

        //���C���J�����̃V�l�}�V�[���u���C���ɍ쐬����CinemachineBlendDefenition��ݒ�
        mainCameraBrain.m_DefaultBlend = blend;

    }
    /// <summary>
    /// ���ݗL���ȃJ�������擾����֐�
    /// </summary>
    /// <returns></returns>

    public CinemachineVirtualCamera GetCurrentCamera()
    {
        return cinemachineVirtualCameras.FirstOrDefault(camera => camera.Priority == 10);
    }

    /// <summary>
    /// �w�肵���J�������擾����֐�
    /// </summary>
    /// <param name="cameraName"></param>
    /// <returns></returns>
    public CinemachineVirtualCamera GetCamera(string cameraName)
    {
        return cinemachineVirtualCameras.FirstOrDefault(camera => camera.Name == cameraName);
    }

    /// <summary>
    /// �J�������w�肵���|�W�V�����Ɉړ�������֐�(cinemachine���g��Ȃ����o)
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="moveSpeed"></param>
    /// <returns></returns>

    public IEnumerator MoveCameraTo(Vector3 destination, float moveSpeed)
    {
        mainCameraBrain.enabled = false;
        while (Vector3.Distance(mainCamera.transform.position, destination) > 0.01f)
        {
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, destination, moveSpeed);
            yield return null;
        }

        mainCamera.transform.position = destination;
    }

    /// <summary>
    /// �J���������Ă������[�e�[�V�����ɉ�]������֐�(cinemachine���g��Ȃ����o)
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="rotateSpeed"></param>
    /// <returns></returns>

    public IEnumerator RotateCameraTo(Quaternion destination, float rotateSpeed)
    {
        mainCameraBrain.enabled = false;
        while (Quaternion.Angle(mainCamera.transform.rotation, destination) > 0.01f)
        {
            mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, destination, rotateSpeed);
            yield return null;
        }

        mainCamera.transform.rotation = destination;
    }
    /// <summary>
    /// ���X�|�[���������ɃJ������߂����߃`�F�b�N�|�C���g�ɐG�ꂽ�Ƃ��̃J�����̖��O��o�^����֐�
    /// </summary>
    public void SetCameraBeforeDie()
    {
        cameraNameBeforeDie = Instance.GetCurrentCamera().gameObject.name;
    }
}
