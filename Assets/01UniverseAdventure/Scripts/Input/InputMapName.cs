using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Input/マップの名前")]
public class InputMapName : ScriptableObject
{
    [SerializeField] string directionInputMap = "Direction";
    [SerializeField] string attackInputMap = "Attack";
    [SerializeField] string jumpInputMap = "Jump";
    [SerializeField] string dashInputMap = "Dash";
    [SerializeField] string hipDropInputMapName = "HipDrop";
    [SerializeField] string pauseInputMapName = "Pause";
    [SerializeField] string decisionInputMapName = "Decision";
    [SerializeField] string goToNextSerifInputMapName = "GoToNextSerif";

    public string DirectionInputMap { get => directionInputMap;}
    public string AttackInputMap { get => attackInputMap; }
    public string JumpInputMap { get => jumpInputMap; }
    public string DashInputMap { get => dashInputMap; }
    public string HipDropInputMapName { get => hipDropInputMapName;}
    public string PauseInputMapName { get => pauseInputMapName; }
    public string DecisionInputMapName { get => decisionInputMapName;}
    public string GoToNextSerifInputMapName { get => goToNextSerifInputMapName; }
}
