using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Animator/トランジション条件データ")]
public class AnimatorTransitionName : ScriptableObject
{
    [SerializeField] string rabitRunBoolName = "IsRunning";

    public string RabitRunBoolName { get => rabitRunBoolName; }
}
