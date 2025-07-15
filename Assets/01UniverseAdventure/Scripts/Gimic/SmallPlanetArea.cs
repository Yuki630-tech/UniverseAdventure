using Cinemachine;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Runtime.InteropServices;

public class SmallPlanetArea : MonoBehaviour
{
    [Tooltip("�J�����̐؂�ւ�����@"), SerializeField] private CinemachineBlendDefinition.Style blendStyle;
    [Tooltip("�؂�ւ���̂ɂ����鎞��"), SerializeField] private float blentTime;
    [Tooltip("�v���C���[�̓����蔻��"), SerializeField] private Collider playerCol;
    [Tooltip("�X���[���X�e�[�W�̃X�^�[�g�n�_�̃^�O��"), SerializeField] private string smallAreaStartTag;
    [Tooltip("�X���[���X�e�[�W�̃X�^�[�g�n�_�̃^�O��"), SerializeField] private string smallAreaEndTag;
    [Tooltip("���s�Ƃ݂Ȃ����ς̍ŏ��l"), SerializeField] private float parallelDot = 0.98f;
    [Tooltip("�����Ƃ݂Ȃ����ς̍ő�l"), SerializeField] private float verticalDot = 0.01f;
    [Tooltip("������������Ă���Ƃ��̃J�����̖��O"), SerializeField] private string upCameraName;
    [Tooltip("�������������Ă���Ƃ��̃J�����̖��O"), SerializeField] private string downCameraName;


    private ReactiveProperty<float> dotBetweenPlayerUpAndWorldUpProperty = new ReactiveProperty<float>();
    [Header("���݂̃v���C���[�̌���"), SerializeField] private PlayerDirection currentDirection;

    CompositeDisposable disposables = new CompositeDisposable();

    public enum PlayerDirection
    {
        Up,
        Down
    }
    

    private void Awake()
    {
        playerCol.OnTriggerEnterAsObservable().Where(other => other.CompareTag(smallAreaStartTag)).Subscribe(_ => StartSmallArea()).AddTo(this);
        playerCol.OnTriggerEnterAsObservable().Where(other => other.CompareTag(smallAreaEndTag)).Subscribe(_ => EndSmallArea()).AddTo(this);
    }

    private void StartSmallArea()
    {
        dotBetweenPlayerUpAndWorldUpProperty.Where(dotBetweenPlayerUpAndWorldUp => IsInRange(dotBetweenPlayerUpAndWorldUp, verticalDot) && currentDirection == PlayerDirection.Down)
            .Subscribe(_ =>
            {
                CameraManager.Instance.ChangeCamera(upCameraName, blendStyle, blentTime);
            }).AddTo(disposables);
        dotBetweenPlayerUpAndWorldUpProperty.Where(dotBetweenPlayerUpAndWorldUp => IsInRange(dotBetweenPlayerUpAndWorldUp, verticalDot) && currentDirection == PlayerDirection.Up)
            .Subscribe(_ =>
            {
                CameraManager.Instance.ChangeCamera(downCameraName, blendStyle, blentTime);
            }).AddTo(disposables);
        dotBetweenPlayerUpAndWorldUpProperty.Where(dotBetweenPlayerUpAndWorldUp => dotBetweenPlayerUpAndWorldUp >= parallelDot).Subscribe(_ =>
        {
            CameraManager.Instance.ChangeCamera(upCameraName, blendStyle, blentTime);
            currentDirection = PlayerDirection.Up;
        }).AddTo(disposables);
        dotBetweenPlayerUpAndWorldUpProperty.Where(dotBetweenPlayerUpAndWorldUp => dotBetweenPlayerUpAndWorldUp <= -parallelDot).Subscribe(_ =>
        {
            CameraManager.Instance.ChangeCamera(downCameraName, blendStyle, blentTime);
            currentDirection = PlayerDirection.Down;
        });
    }

    private bool IsInRange(float a, float b)
    {
        return Mathf.Abs(a) <= b;
    }

    private void EndSmallArea()
    {
        disposables.Dispose();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        dotBetweenPlayerUpAndWorldUpProperty.Value = Vector3.Dot(playerCol.transform.up, Vector3.up);
        //DebugLog.Log($"�����H: {IsInRange(dotBetweenPlayerUpAndWorldUpProperty.Value, verticalDot)}");
    }
}
