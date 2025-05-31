using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDrawnIntoBlackHole : MonoBehaviour
{
    [Tooltip("�����̔��a"), SerializeField] float startRadius = 10;
    [Tooltip("��]�p�x�̏����l"), SerializeField] float startAngle = 0;
    [Tooltip("��]���x�̏����l(+���E���A-�������)"), SerializeField] float startRotateSpeed;
    [Tooltip("��]�̍ō����x(+���E���A-�������)"), SerializeField] float maxRotateSpeed = 1;
    [Tooltip("��]�̉����x(+���E���A-�������Bstartrotate�ƕ������t�ɂ���ƌ���)"), SerializeField] float rotateAcceleration;
    [Tooltip("���a���k�������鑬�x"), SerializeField] float radiusSpeed;
    [Tooltip("��]�̒��S���W(����)"), SerializeField] Vector3 center;
    [Tooltip("�z�����܂�Ă������x"), SerializeField] float drawnSpeed = 5;

    //�z�����܂�Ă����ԂɍX�V����Ă����ϐ��Q
    float radius; 
    [SerializeField] float angle;
    float rotateSpeed;
    float posZ;
    /// <summary>
    /// ��]�J�n���̍��W
    /// </summary>
    Vector3 startPos; 

    /// <summary>
    /// ��]�̒��S����u���b�N�z�[���ւ̕����x�N�g��(z��)
    /// </summary>
    Vector3 directionFromCenterToBlackHole;

    /// <summary>
    /// �v���C���[�����]�̒��S�ւ̕����x�N�g��(���S�����肷�邽�߂̃x�N�g��)
    /// </summary>
    Vector3 directionFromPlayerToCenter;
    /// <summary>
    /// ���S����v���C���[�ւ̕����x�N�g��(x��)
    /// </summary>
    Vector3 directionFromCenterToPlayer;

    /// <summary>
    /// y��
    /// </summary>
    Vector3 directionY; 
    
    [Tooltip("�z�����܂���ƂȂ�u���b�N�z�[����transform"), SerializeField] Transform targetTransform;

    public Vector3 Center { get => center; }
    public Vector3 DirectionFromCenterToBlackHole { get => directionFromCenterToBlackHole; }

    /// <summary>
    /// �u���b�N�z�[���ւ̋z�����܂���J�n����֐�
    /// </summary>
    /// <param name="setTransform">�z�����܂���ƂȂ�blackhole</param>
    // Start is called before the first frame update
    public void StartGetDrawn(Transform setTransform)
    {
        targetTransform = setTransform;
        //���ꂼ��̒l�������l�ɐݒ�
        rotateSpeed = startRotateSpeed;
        radius = startRadius;
        angle = startAngle;
        posZ = 0;
        startPos = transform.position;

        //�v���C���[����u���b�N�z�[���ւ̕����x�N�g���ƃ��[���h��Ԃ̏�����Ƃ̊O�ς̕�����radius�̋��������i�񂾈ʒu����]�̒��S�Ƃ��Đݒ肷��
        var direction = targetTransform.position - startPos;
        direction = direction.normalized;
        directionFromPlayerToCenter = Vector3.Cross(direction, Vector3.up).normalized;
        center = startPos + directionFromPlayerToCenter * radius;

        //��]����ۂɗ��p������W�n�ɂ���x���𒆐S����v���C���[�ւ̕����x�N�g���Az������]�̒��S����u���b�N�z�[���ւ̕����x�N�g���ɐݒ肷��
        directionFromCenterToPlayer = transform.position - center;
        directionFromCenterToPlayer = directionFromCenterToPlayer.normalized; //x��
        directionFromCenterToBlackHole = (targetTransform.position - center).normalized; //z��

        //��]�̍��W�n�ɂ�����z����x���̊O�ς�y���Ƃ���
        directionY = Vector3.Cross(directionFromCenterToBlackHole, directionFromCenterToPlayer); //y��

    }

    // Update is called once per frame
    public void UpdateGetDrawn()
    {
        GetDrawnMove();
        ChangeAngleZAndRadius();
        
    }

    /// <summary>
    /// ���W���X�V����֐��B
    /// </summary>

    void GetDrawnMove()
    {
         var x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
         var y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        �@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@//x��                       //y��                      //z��
        transform.position = center + directionFromCenterToPlayer * x + directionY * y + directionFromCenterToBlackHole * posZ;
        DebugLog.Log("( " + x + " , " + y + " ) ");
    }

    /// <summary>
    /// �p�x�𖈕b��]���x�����A��]���x�𖈕b�����x���������A���a�����������u���b�N�z�[���̕����Ɉړ�������֐��B
    /// </summary>

    void ChangeAngleZAndRadius()
    {
        angle += rotateSpeed * Time.deltaTime;
        rotateSpeed += rotateAcceleration * Time.deltaTime;
        rotateSpeed = Mathf.Clamp(rotateSpeed, 0, maxRotateSpeed);
        radius -= radiusSpeed * Time.deltaTime;
        radius = Mathf.Clamp(radius, 0, startRadius);

        posZ += drawnSpeed * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        if (targetTransform == null) return;
        var radius = startRadius;
        var startPos = transform.position;
        var direction = targetTransform.position - startPos;
        direction = direction.normalized;

        var directionToCenter = Vector3.Cross(direction, Vector3.up).normalized;
        var center = startPos + directionToCenter * radius;
        var directionFromCenterToPlayer = transform.position - center;
        directionFromCenterToPlayer = directionFromCenterToPlayer.normalized;
        var directionFromCenterToBlackHole = (targetTransform.position - center).normalized;
        var directionX = Vector3.Cross(directionFromCenterToPlayer, directionFromCenterToBlackHole);
        Gizmos.color = Color.green;
        if (targetTransform != null)
            Gizmos.DrawRay(startPos, direction * Vector3.Distance(startPos, targetTransform.position));

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(center, directionFromCenterToBlackHole * Vector3.Distance(targetTransform.position, center));
        Gizmos.color = Color.red;
        
        Gizmos.DrawRay(startPos, directionToCenter.normalized * radius);
        var wireSphereRadius = 0.5f;
        Gizmos.DrawWireSphere(center, wireSphereRadius);
        Gizmos.color = Color.black;
        Gizmos.DrawRay(center, directionX * radius);


    }
}
