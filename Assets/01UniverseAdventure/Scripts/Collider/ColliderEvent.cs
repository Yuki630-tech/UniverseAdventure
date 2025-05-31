using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderEvent : MonoBehaviour
{
    [Tooltip("コライダーのトリガーに触れたときに発生させるイベント"), SerializeField] UnityEvent<Collider> onTriggerEvent;

    
    private void OnTriggerEnter(Collider other)
    {
        onTriggerEvent.Invoke(other);
    }
}
