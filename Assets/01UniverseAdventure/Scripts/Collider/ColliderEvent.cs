using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderEvent : MonoBehaviour
{
    [Tooltip("�R���C�_�[�̃g���K�[�ɐG�ꂽ�Ƃ��ɔ���������C�x���g"), SerializeField] UnityEvent<Collider> onTriggerEvent;

    
    private void OnTriggerEnter(Collider other)
    {
        onTriggerEvent.Invoke(other);
    }
}
