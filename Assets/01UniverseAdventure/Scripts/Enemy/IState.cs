using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<T> where T : class
{
    public void Enter(T owner);
    public void Update(T owner, float deltaTime);
    public void Exit(T owner);
}
