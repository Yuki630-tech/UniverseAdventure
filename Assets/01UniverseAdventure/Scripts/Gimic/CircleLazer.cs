using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CircleLazer : MonoBehaviour
{
    [Tooltip("�T�[�N���̔��a"), SerializeField] float radius;
    [Tooltip("�T�[�N�����[�U�[���L����ő唼�a"), SerializeField] float maxRadius;
    [Tooltip("�~�̒��_�̐�"), SerializeField] int numberOfVertex = 100;
    [Tooltip("�~�`�̃��[�U�[��`��lineRenderer"), SerializeField] LineRenderer lineRenderer;
    [Tooltip("���[�U�[�ɉ����������蔻��"), SerializeField] LazerCollider lazerCollider;
    [SerializeField] float spreadSpeed = 2f;
    [Tooltip("�L���邾�����f���ɉ��������������邩"), SerializeField] bool isOnlySpread;
    [SerializeField] GameObject planet;
    [Tooltip("���C�U�[�̓_���f�����痣�ꂷ�������Ƃ����m����ŒZ����"),SerializeField] float limitedDistance;
    [Tooltip("���C�U�[����������y�������̋���"), SerializeField] float destroySizeDistanceY;
    List<RaycastHit> hits = new List<RaycastHit>();
    float initializeRadius = 0.01f;
    /// <summary>
    /// ������Ȃ���L�����Ă������[�U�[�ɂ��āA�ŏ��͔��a���Z�݂̂��s���B���̌�n�ʂ��痣��Ă����ɂ��
    /// �L����X�s�[�h���x���Ȃ��Ă��܂��̂œr������L����Ȃ��牺�����Ă����悤�ɂ���B�����Ŋg��݂̂��s��
    /// �ꍇ���̃t���O��true�ɂȂ�
    /// </summary>
    bool isSpread;

    /// <summary>
    /// ������Ȃ���L�����Ă������[�U�[��y���W4
    /// </summary>
    float posY;

    /// <summary>
    /// ���[�U�[�̊e���_�̒n�ʂƂ̋���
    /// </summary>
    Vector3 offset;


    // Start is called before the first frame update
    void Start()
    {
        radius = initializeRadius;
        //lineRender�̓��[�v�����邽�ߍ�肽�����_�ɂ�������������������_��p�ӂ���
        lineRenderer.positionCount = numberOfVertex + 1;

        //�R���C�_�[�̒��_��ݒ�
        lazerCollider.SetColNumber(numberOfVertex);
    }

    // Update is called once per frame
    void Update()
    {
        //�L�����Ă����݂̂̃��[�U�[
        if (isOnlySpread)
        {
            OperateSpreadCircle();
        }

        //�L����Ȃ��牺�����Ă������[�U�[
        else
        {
            SwitchSpreadMode();
            OperateMoveThroghPlanetCircle();
        }

        DrawCircle();
    }

    /// <summary>
    /// ���[�U�[�̍L����ő唼�a�Ƃǂ�planet�ɉ����ă��[�U�[�𓮂�������o�^����֐�
    /// </summary>
    /// <param name="setRadius">�ő唼�a</param>
    /// <param name="setPlanet">�Ώۂ�planet�I�u�W�F�N�g</param>
    public void SetLazerSetting(float setRadius, GameObject setPlanet, float setLimitedDistance, float setStartDownSizeDistanceY)
    {
        maxRadius = setRadius;
        planet = setPlanet;
        limitedDistance = setLimitedDistance;
        destroySizeDistanceY = setStartDownSizeDistanceY;
    }

    /// <summary>
    /// �L���邾���̃��[�U�̏���
    /// </summary>
    void OperateSpreadCircle()
    {
        //�ő唼�a�܂Ń��[�U�[���L����
        if (radius < maxRadius)
        {
            radius += Time.deltaTime * spreadSpeed;
            posY = 0f;
        }

        //�ő唼�a�ɒB�����烌�[�U�[������
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// �f���ɉ����ē������[�U�[�ɂ��čL���鋓���݂̂��s����ԂƉ��ɉ������Ă����Ȃ���L���鋓���������ԂƂ�؂�ւ���֐�
    /// </summary>
    void SwitchSpreadMode()
    {
        //���[�U�̊e���_����f�������Ƀ��C���΂����Ƃ���hit��񂻂ꂼ��ɂ��Ĕ���
        foreach(var hit in hits)
        {
            //���ꂼ���distance��limitedDistance���Z���ꍇ�͊g��̂�
            if(hit.distance <= limitedDistance)
            {
                isSpread = true;
            }

            //�~���L���肷�����牺�ɉ������Ă��������ɐ؂�ւ���
            else
            {
                isSpread = false;
                break;
            }
        }
    }

    /// <summary>
    /// �L����Ȃ��牺�����Ă������[�U�[�̏���
    /// </summary>
    void OperateMoveThroghPlanetCircle()
    {
        //�����g�傷��
        if (isSpread)
        {
            radius += Time.deltaTime * spreadSpeed;
            posY = 0f;
        }

        //�g�債�Ȃ��牺�~
        else
        {
            posY -= Time.deltaTime * spreadSpeed;
            if(posY >= -destroySizeDistanceY)
            {
                radius += Time.deltaTime * spreadSpeed;

            }

            else
            {
                Destroy(gameObject);
            }

        }
    }

    /// <summary>
    /// �~�`�̃��[�U�[��`���֐�
    /// </summary>
    void DrawCircle()
    {
        
        var segmentAngle = 360f / numberOfVertex;
        for (int i = 0; i <= numberOfVertex; i++)
        {
            Vector3 linePos = Vector3.zero;
            var angle = segmentAngle * i * Mathf.Deg2Rad;
            //x = cos(angle) �~ ���a   z = sin(angle) �~ ���a
            var x = Mathf.Cos(angle) * radius;
            var y = posY;
            var z = Mathf.Sin(angle) * radius;

            linePos = new Vector3(x, y, z);

            //�L���邾���łȂ����ɍ~��Ă����^�C�v�̃��[�U�[�̏ꍇ
            if (!isOnlySpread)
            {
                
                var worldPos = transform.position + linePos;

                //���ꂼ��̒��_�̍��W���f����̒n�ʂ���ǂꂾ�����ꂽ�����擾���Ă��̕��ړ�������
                offset = CheckGroundOffset(worldPos);
                var direction = (planet.transform.position - worldPos).normalized;

                //���̂܂܂��ƒn�ʂɖ��܂��Ă��܂��̂ŏ���������ɂ�����B
                var correction = -direction;

                linePos = linePos + offset + correction;
            }

            lineRenderer.SetPosition(i, linePos);
        }

    }

    /// <summary>
    /// �n�ʂƃ��[�U�[�̒��_�Ƃ̋������擾����
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    Vector3 CheckGroundOffset(Vector3 origin)
    {
        var direction = (planet.transform.position - origin).normalized;
        RaycastHit hit;

        //�e���_�̈ʒu����f���Ɍ����ă��C���΂��f���̕\�ʂƂ̊Ԃ̋������擾
        if (Physics.Raycast(origin, direction, out hit))
        {
            if (hit.collider.gameObject == planet)
            {
                hits.Add(hit);
                return direction * hit.distance;
            }

            //���C�Ƀv���l�b�g��������Ȃ��ꍇ��offset�̊Ԃ����̂܂ܕԂ�
            else
            {
                return offset;
            }

        }

        else
        {
            return offset;
        }
    }


}
