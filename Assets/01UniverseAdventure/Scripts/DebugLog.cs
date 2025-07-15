using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugLog
{
    public static void Log(string log)
    {
#if UNITY_EDITOR
        Debug.Log(log);
#endif
    }
}
