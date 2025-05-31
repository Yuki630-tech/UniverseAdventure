using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniRx;
#if UNITY_EDITOR
[ExecuteInEditMode]
/// <summary>
/// �S�����]�����Ă����������킩��₷�����邽�߂̃N���X
/// </summary>
public class DebugForward : MonoBehaviour
{
    /// <summary>
    /// debug�_�̘f���\�ʂ���̋���
    /// </summary>
    Vector3 offset;
    [Tooltip("�M�Y���̒��_��"), SerializeField] int vertexNum = 100;
    [Tooltip("�M�Y���Ԃ̋���"), SerializeField] float vertexInterval = 3f;
    /// <summary>
    /// �X�V�O�̃I�u�W�F�N�g�̍��W
    /// </summary>
    Vector3 lastPosition;
    [Tooltip("��]�����ۂɓo�^�����]�O�̃��[�e�[�V����"), SerializeField] Quaternion lastRotation;

    /// <summary>
    /// debug�_�̃��X�g
    /// </summary>
    List<Vector3> debugVertexSet = new List<Vector3>();
    [Tooltip("�M�Y����Y�킹��Ώۂ̃I�u�W�F�N�g"),SerializeField] GameObject planet;

    private void Awake()
    {
        StartCoroutine(Sort());

    }
    private void OnValidate()
    {
        
        //���W�Ɖ�]�����L���b�V��
        lastPosition = transform.position;�@
        lastRotation = transform.rotation;

        //undo�����Ƃ���ray��������悤�ɂ���
        Undo.undoRedoPerformed += OnUndoRedo;
        
       
    }
    /// <summary>
    /// ray��n�ʂɉ��킹��悤�ɂ���֐�
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    Vector3 CheckGroundOffset(Vector3 origin)
    {
        //�w��̍��W����gravity�N���X�ɓo�^����Ă���planet�I�u�W�F�N�g�Ɍ�������Ray���΂�
        var direction = (planet.transform.position - origin).normalized;
        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit))
        {
            //�q�b�g�����I�u�W�F�N�g��gravity�ɓo�^����Ă���planet�I�u�W�F�N�g�Ȃ�q�b�g�����ꏊ�܂ł̋�����Ԃ�
            if (hit.collider.gameObject == planet)
            {
                return direction * hit.distance;
            }

    �@�@//����ȊO�Ȃ�offset�̒l�����̂܂ܕԂ�
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

    private void OnDrawGizmos()
    {
        //�Đ����[�h���APlanet�I�u�W�F�N�g���Ȃ��ꍇ�̓M�Y����`���Ȃ�
        if (Application.isPlaying || planet == null) return;
        //�G���ړ��A��]���������ɃM�Y�����s���R�ɂȂ�Ȃ��悤�ɉ�]�O�ɑ��݂����M�Y���̒��_���폜����
        if (transform.rotation != lastRotation)
        {

            debugVertexSet.Clear();
            lastRotation = transform.rotation;

        }

        if (transform.position != lastPosition)
        {
            debugVertexSet.Clear();
            lastPosition = transform.position;

        }
        Gizmos.color = Color.red;

        //�f���ɉ��������C�̃M�Y����`��
        for (int i = 0; i < debugVertexSet.Count - 3; i++)
        {
            var direction = debugVertexSet[i + 1] - debugVertexSet[i];
            direction = direction.normalized;
            var distance = Vector3.Distance(debugVertexSet[i], debugVertexSet[i + 1]);
            Gizmos.DrawRay(debugVertexSet[i], direction * distance);
        }
    }

    /// <summary>
    /// UnDo�����Ƃ��Ƀ|�C���g���폜���ăM�Y�����d�Ȃ�Ȃ��悤�ɂ���
    /// </summary>
    void OnUndoRedo()
    {
        if (gameObject != null && Selection.Contains(gameObject))
        {
            debugVertexSet.Clear();
            SetPoints();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator Sort()
    {
        if(Application.isPlaying) yield break;
        while (true)
        {
            SetPoints();
            yield return new WaitUntil(() => transform.rotation != lastRotation || transform.position != lastPosition);
        }
    }

    void SetPoints()
    {
        for (int i = 0; i < vertexNum; i++)
        {
            var pos = transform.forward * i * vertexInterval;

            var worldPos = transform.position + pos;
            offset = CheckGroundOffset(worldPos);
            var correction = (worldPos - planet.transform.position).normalized;
            worldPos = worldPos + offset + correction;
            debugVertexSet.Add(worldPos);

        }

        debugVertexSet.Sort((a, b) =>
        {
            float distA = Vector3.Distance(a, transform.position);
            float distB = Vector3.Distance(b, transform.position);
            return distA.CompareTo(distB);
        });
    }


}
#endif