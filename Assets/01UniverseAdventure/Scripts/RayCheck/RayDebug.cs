using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using System.Net.NetworkInformation;

public static class RayDebug
{
    public static void DrawRayGizmo(Vector3 origin, Vector3 direction, Color debugColor, float distance = Mathf.Infinity)
    {
        Gizmos.color = debugColor;
        Gizmos.DrawRay(origin, direction * distance);
    }
    public static void DrawSphereGizmo(Vector3 origin, float radius, Vector3 direction, Color debugColor, float distance = Mathf.Infinity)
    {
        Gizmos.color = debugColor;
        Gizmos.DrawRay(origin, direction * distance);
        Vector3 endPos = origin + direction * distance;
        Gizmos.DrawWireSphere(endPos, radius);
    }

   
}
