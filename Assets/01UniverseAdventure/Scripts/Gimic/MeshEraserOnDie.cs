using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class MeshOnDie : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.OnBlackHoleObservable.Subscribe(_ => GetComponent<MeshRenderer>().enabled = false).AddTo(gameObject);
        GameManager.Instance.OnPlayerRestartObservable.Subscribe(_ => GetComponent<MeshRenderer>().enabled = true).AddTo(gameObject);
    }
}
