using Cinemachine;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using System.Runtime.InteropServices;

public class SmallPlanetArea : MonoBehaviour
{
    [Tooltip("カメラの切り替える方法"), SerializeField] private CinemachineBlendDefinition.Style blendStyle;
    [Tooltip("切り替えるのにかける時間"), SerializeField] private float blentTime;
    [Tooltip("プレイヤーの当たり判定"), SerializeField] private Collider playerCol;
    [Tooltip("スモールステージのスタート地点のタグ名"), SerializeField] private string smallAreaStartTag;
    [Tooltip("スモールステージのスタート地点のタグ名"), SerializeField] private string smallAreaEndTag;
    [Tooltip("平行とみなす内積の最小値"), SerializeField] private float parallelDot = 0.98f;
    [Tooltip("垂直とみなす内積の最大値"), SerializeField] private float verticalDot = 0.01f;
    [Tooltip("上方向を向いているときのカメラの名前"), SerializeField] private string upCameraName;
    [Tooltip("下方向を向いているときのカメラの名前"), SerializeField] private string downCameraName;


    private ReactiveProperty<float> dotBetweenPlayerUpAndWorldUpProperty = new ReactiveProperty<float>();
    [Header("現在のプレイヤーの向き"), SerializeField] private PlayerDirection currentDirection;

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
        //DebugLog.Log($"垂直？: {IsInRange(dotBetweenPlayerUpAndWorldUpProperty.Value, verticalDot)}");
    }
}
