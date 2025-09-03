using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<TEnum, TType> where TEnum : Enum where TType : class 
{
    private Dictionary<TEnum, IState<TType>> states = new Dictionary<TEnum, IState<TType>>();
    private IState<TType> currentState;
    public TEnum CurrentStateName { get; private set; }

    public void AddState(TEnum stateName, IState<TType> state)
    {
        states[stateName] = state;
    }

    public void ChangeState(TEnum setState, TType owner)
    {
        if(currentState != null)
        {
            currentState.Exit(owner);
        }
        currentState = states[setState];
        if(currentState != null)
        {
            currentState.Enter(owner);
        }
        CurrentStateName = setState;
    }

    public void Update(TType owner, float deltaTime)
    {
        currentState?.Update(owner, deltaTime);
    }
}
