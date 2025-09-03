using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastObjActivator : MonoBehaviour
{
    [Tooltip("有効化するオブジェクト"), SerializeField] private GameObject activateObj;

    private void Awake()
    {
        activateObj.SetActive(false);
    }

    public void ActivateObj()
    {
        activateObj.SetActive(true);
    }
}
