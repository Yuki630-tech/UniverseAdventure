using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public abstract class ItemBase<TOther> : MonoBehaviour where TOther : class
{
    [Tooltip("Collider‚ÌƒŠƒXƒg"), SerializeField] private List<Collider> cols = new List<Collider>();
    private void Awake()
    {
        foreach(var col in cols)
        {
            col.OnTriggerEnterAsObservable().Subscribe(other => OnTriggerEnterEvent(other));
        }
    }
    protected abstract void GetItem(TOther getter);
    private void OnTriggerEnterEvent(Collider other)
    {
        TOther getter = other.GetComponent<TOther>();
        
        if(getter != null)
        {
            GetItem(getter);
            gameObject.SetActive(false);
        }
    }
}
