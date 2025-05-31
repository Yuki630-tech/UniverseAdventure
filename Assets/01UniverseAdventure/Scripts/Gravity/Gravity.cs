using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Threading;
using System;

public class Gravity : MonoBehaviour
{
    [Tooltip("�d�͂������邽�߂�Rigidbody"), SerializeField] Rigidbody rb;
    [Tooltip("�d�͌��ƂȂ�f��"), SerializeField] GameObject planetObj;
    [Tooltip("���f�����𔻒f���邽�߂̘f���ɂ��Ă���R���|�[�l���g"), SerializeField] Planet planet;

    [Tooltip("�d�͕ϊ����u�ɐG�ꂽ�Ƃ��ɓ����Ȃ����邽�߂Ɉړ��𐧌䂷��R���|�[�l���g���擾���Ă���"), SerializeField] Movable movable;

    [Tooltip("�S���̏ꍇ�]�����Ăق����̂ŏ�������Œ肵�Ȃ��悤�ɂ������B���̂��ߏ�����Œ�̃L�����N�^�[���ǂ��������肷��t���O��p�ӂ��Ă���"), SerializeField]
    bool isCharacter;
    [Tooltip("�d�͂̑傫��"), SerializeField] float gravity;

    [Tooltip("�d�ʕ����ւ̉�]�̃X�s�[�h"), SerializeField] float rotateSpeedTowardGravityUp = 120;

    [Tooltip("�d�͐؂�ւ��̌��ʂ��󂯎��Ȃ�����"), SerializeField] float disableTime = 0.5f;


    /// <summary>
    /// Player�ɑ΂���d�͌��ł���f���̌���
    /// </summary>
    Vector3 direction;

    //Ray���ڐG�����f���̃|���S���̖@��
    [Tooltip("�ڐG�����f���̖@��"), SerializeField] Vector3 normalVec = Vector3.zero;

    [Header("�v���C���[���d�͐؂�ւ��G���A�ɂ��邩�ǂ���"), SerializeField] bool isInGravityChangeArea;

    [Header("�d�͐؂�ւ��G���A�ɂ����Ĉ�莞�Ԃ��Ƃɏd�͂�ω�������@�\�����R���|�[�l���g"), SerializeField] GravityChanger gravityChanger;

    [Header("�d�͐؂�ւ��G���A�ɓ��������̏�����x�N�g���̏����l"), SerializeField] Vector3 up;

    //UniTask�L�����Z��
    CancellationTokenSource cancellationTokenSource;
    CancellationToken token;

    /// <summary>
    /// �v���C���[�����ݐG�ꂽ�΂����GravityChanger�B�V����GravityChanger�ɐG���܂ł��̋@�\���~���邽�߂Ɏ擾���ۑ����Ă����B
    /// �V���ɕʂ�GravityChanger�ɐG�ꂽ�Ƃ��ɂ��̐V���Ȃ��̂��o�^����A�O��GravityChanger�ւ̌��ʂ���������
    /// </summary>
    [SerializeField] GameObject currentGravityTrap;
    [Tooltip("�d�͐؂�ւ��G���A�ɂ����ďd�͂����]�������ɒn�ʂɖ��܂��Ă��܂��̂ł��̒l�̕������d�͕����Ɉړ�������"), SerializeField] 
    float moveDistanceWhenGravityInverse = 0.5f;

    /// <summary>
    /// ���Ԃ��Ƃɏd�͂�ω�������@�\��Disposable.
    /// ���̋@�\��͈͊O�ɏo���Ƃ���Dispose()�ł���悤�ɕێ����Ă���
    /// </summary>
    IDisposable gravityChangeDisposable;

    /// <summary>
    /// �ڐG�����f���̖@��
    /// </summary>
    public Vector3 NormalVec { get => normalVec; }

    /// <summary>
    /// �d�͌��ƂȂ�f��
    /// </summary>
    public GameObject PlanetObj { get => planetObj; }

    /// <summary>
    /// ���f�����𔻒f���邽�߂̘f���ɂ��Ă���R���|�[�l���g
    /// </summary>
    public Planet Planet { get => planet; }

    /// <summary>
    /// ���Ԃ��Ƃɏd�͂�ω�������@�\������Disposable.
    /// ���̋@�\��͈͊O�ɏo���Ƃ���Dispose()�ł���悤�ɕϐ��Ƃ��Ď擾���Ă���
    /// </summary>
    public IDisposable GravityChangeDisposable { get => gravityChangeDisposable; }

    private void Awake()
    {
        //UniTask�̃L�����Z���[�V�����g�[�N����p��
        cancellationTokenSource = new CancellationTokenSource();
        token = cancellationTokenSource.Token;
        //�f���I�u�W�F�N�g���o�^����Ă���΃A�^�b�`����Ă���Planet�R���|�[�l���g��o�^
        if (planetObj != null)
        {
            planet = planetObj.GetComponent<Planet>();
        }
        //�v���C���[�����񂾂Ƃ��A�Q�[�����N���A��������GravityChanger�ɂ��@�\��j��
        GameManager.Instance?.OnPlayerDieObservable.Subscribe(_ => GravityChangeDisposable?.Dispose()).AddTo(this);
        GameManager.Instance?.OnGameClearOrOverObservable.Subscribe(_ => GravityChangeDisposable?.Dispose()).AddTo(this);
    }

    // Start is called before the first frame update
    void Start()
    {

        rb.useGravity = false;

    }

    private void Update()
    {
        Attract();
        PlanetRayCheck();
    }

    /// <summary>
    /// �d�͂𓭂����A�d�͕����ɃI�u�W�F�N�g����������֐�
    /// </summary>

    public void Attract()
    {
        Vector3 gravityUp = normalVec;

        Vector3 bodyUp = transform.up;

        if (rb != null)
        {
            //�Q�[���|�[�Y���͂����铮�����~�߂邽�ߏd�͂������Ȃ��悤�ɂ��A���x��0�ɂ���
            if (GameManager.Instance != null && GameManager.Instance.IsPausing && isCharacter)
            {
                if (!rb.isKinematic)
                    rb.velocity = Vector3.zero;
            }
            else
            {
                rb.AddForce(gravityUp * gravity);
            }

        }

        //�L�����N�^�[�Ȃ�NormalVec�̕����ɃI�u�W�F�N�g�̏�����������悤�ɂ���
        if (isCharacter)
        {
            Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * transform.rotation;

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeedTowardGravityUp * Time.deltaTime);
        }

    }

    /// <summary>
    /// gravity��NormalVec�܂���Planet��o�^����֐�
    /// </summary>
    /// <param name="setGravity"></param>

    public void SetGravity(Gravity setGravity)
    {
        //�o�^���鑤��Gravity�ɘf�����o�^����Ă��Ȃ����NormalVec���A����Ă���Θf�������g�ɓo�^����
        if (setGravity.Planet == null)
        {
            normalVec = setGravity.normalVec;
        }

        else
        {
            planetObj = setGravity.PlanetObj;
            planet = setGravity.Planet;
        }

    }

    /// <summary>
    /// Planet��ݒ肷��֐��B�����Ŏw�肵���̒�����ЂƂÂf����I�яo���āA���ݏd�͌��Ƃ���Gravity�ɓo�^����Ă���f���ƈ�v���邩�ǂ����R�����A
    /// ��v���Ȃ���΂��̘f�����d�͂̔������Ƃ���Gravity�ɓo�^����B
    /// </summary>
    /// <param name="setObject"></param>
    /// <param name="planets">�f���̌��</param>
    /// <param name="hasImmovableEffect"></param>
    /// <param name="hasWaitUntilGroundedEffect"></param>
    /// <returns></returns>
    public async UniTask ChoosePlanet(GameObject setObject, List<GameObject> planets, bool hasImmovableEffect, bool hasWaitUntilGroundedEffect)
    {
        try
        {
            //�G�ꂽ�΂����GravitychangeTrap�ɍĂѐG�ꂽ�Ƃ��͋@�\���Ȃ��悤�ɂ���
            if (currentGravityTrap == setObject) return;
            //GravityChangeTrap�o�^
            currentGravityTrap = setObject;

            for (int i = 0; i < planets.Count; i++)
            {
                //���̒�����I�΂ꂽ�f�������ݎ��g��Gravity�ɓo�^����Ă���f���ƈ�v����ꍇ�͐R�����p��
                if (planets[i] == planetObj) continue;
                //��v���Ȃ���΂��̌��̘f�����d�͌��Ƃ��ēo�^���ă��[�v�𔲂���B
                else
                {
                    planetObj = planets[i];
                    planet = planetObj.GetComponent<Planet>();
                    break;
                }
            }

            //���n����܂Ńv���C���[�������Ȃ��悤�ɂ���
            if (hasImmovableEffect)
            {
                movable.GoToImmovable();
            }

            //�f���o�^�サ�΂炭���������Ă����Ȃ��ƕ����񂱂̊֐����Ă΂�Ĉ�u�Řf�����؂�ւ��Ƃ����o�O���N����̂�
            //��莞�Ԗ���������B
            await UniTask.Delay(System.TimeSpan.FromSeconds(disableTime), cancellationToken: token);
            if (movable != null && hasImmovableEffect)
            {
                if (hasWaitUntilGroundedEffect)
                {
                    await UniTask.WaitUntil(() => movable.IsGround);
                }
                movable.GoToMovable();
            }
            //GravityChangeTrap
            currentGravityTrap = null;
        }

        catch (OperationCanceledException)
        {
            DebugLog.Log("�r���Œ��f���܂���");
        }
    }

    /// <summary>
    /// �d�͌��ł���f���̕����Ƀ��C���΂���NormalVec��ݒ肷��֐��B�f����ɂ��Ȃ��ꍇ�͋@�\���Ȃ��B
    /// </summary>
    public void PlanetRayCheck()
    {
        if (planetObj == null) return;
        //�f���̕����Ƀ��C���΂��A���m�����@���x�N�g����NormalVec�Ƃ��ēo�^�B
        direction = (planetObj.transform.position - transform.position).normalized;

        RaycastHit hit;
        Ray ray = new Ray(transform.position, direction);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Planet"))
            {
                normalVec = hit.normal;
            }

        }

    }

    /// <summary>
    /// planet���Ȃ��󋵂ŏd�͂�ݒ肷��֐��B
    /// </summary>
    /// <param name="setObject"></param>
    /// <param name="hasImmovableEffect"></param>
    /// <param name="hasWaitUntilGroundedEffect"></param>
    /// <param name="direction"></param>
    /// <returns></returns>

    public async UniTask SetGravity(GameObject setObject, bool hasImmovableEffect, bool hasWaitUntilGroundedEffect, Vector3 direction)
    {
        try
        {
            //�G�ꂽ�΂����GravityChangeTrap�ɍĂѐG�ꂽ�Ƃ��ɂ͋@�\���Ȃ��悤�ɂ���
            if (setObject == currentGravityTrap) return;
            //GravityChangeTrap�o�^
            currentGravityTrap = setObject;
            //�f���ɏ]�����d�͂ł͂Ȃ��̂ŏd�͌��̘f���͋�ɂ���
            planetObj = null;
            planet = null;

            //NormalVec�������Ɏw�肵�����̂ɐݒ�
            normalVec = direction;
            //�d�͂�؂�ւ���ۂɈړ����~�߂�ݒ�ɂȂ��Ă���ꍇ�ɂ͈ړ����~�߂�
            if (hasImmovableEffect)
            {
                movable?.GoToImmovable();
            }
            await UniTask.Delay(System.TimeSpan.FromSeconds(disableTime), cancellationToken: token);
            if (movable != null)
            {
                //�d�͐؂�ւ����u�ɐG�ꂽ��󒆂ɂ���Ԃɕʂ̏d�͐؂�ւ����u�ɐG���\��������ꍇ�A���n�܂ŏd�͂��ς��Ȃ��ƍ���̂�
                //���n�܂ő҂��҂��Ȃ��������߂�t���O��p�ӂ���B
                if (hasWaitUntilGroundedEffect)
                {
                    await UniTask.WaitUntil(() => movable.IsGround);

                }


                if (hasImmovableEffect)
                {
                    movable?.GoToMovable();
                }
            }

            currentGravityTrap = null;
        }

        catch (OperationCanceledException)
        {
            DebugLog.Log("���f����܂���");
        }
    }

    /// <summary>
    /// ��莞�Ԃ��Ƃɏd�͂��؂�ւ��悤�ɂ���֐�
    /// </summary>
    /// <param name="setChanger"></param>
    public void SetGravityChanger(GravityChanger setChanger)
    {
        //GravityChanger�o�^
        gravityChanger = setChanger;
        //���g�̏��������ƂȂ������Ƃ��ēo�^
        up = transform.up;

        //��ƂȂ������x�N�g����GravityChanger��GravityDirection(��莞�Ԃ��Ƃ�1 / -1�ɐ؂�ւ��UniRx��ReactiveProperty)
        //�������Ĉ�莞�Ԃ��Ƃɏd�͂��؂�ւ��悤�ɂ���B
        gravityChangeDisposable = gravityChanger?.GravityDirection.Subscribe(direction =>
        {
            normalVec = up * direction;
            transform.position += -normalVec * moveDistanceWhenGravityInverse;
            transform.rotation = Quaternion.Euler(0f, 180, 0f) * transform.rotation;
        }).AddTo(this);

    }

    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
    }
}
