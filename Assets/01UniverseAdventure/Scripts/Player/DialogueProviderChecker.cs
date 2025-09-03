using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueProviderChecker : MonoBehaviour
{
    [Header("会話をするオブジェクトのチェッカーに関する設定")]
    [Tooltip("レイの始点"), SerializeField] private Vector3 startingPoint;
    [Tooltip("レイの半径"), SerializeField] private float radius;
    [Tooltip("レイの長さ"), SerializeField] private float length;
    
    public IDialogueProvider GetDialogueProvider()
    {
        Vector3 origin = Vector3Convert.ConvertToWorld(transform.position, transform, startingPoint);
        RaycastHit hit;
        if(Physics.SphereCast(origin, radius, transform.forward, out hit, length))
        {
            IDialogueProvider dialogueProvider = hit.collider.GetComponent<IDialogueProvider>();
            if (dialogueProvider != null)
            {
                return dialogueProvider;
            }

            else
            {
                return null;
            }
        }

        else
        {
            return null;
        }
    }

    private void OnDrawGizmos()
    {
        RayDebug.DrawSphereGizmo(Vector3Convert.ConvertToWorld(transform.position, transform, startingPoint), radius, transform.forward, Color.red, length);
    }
}
