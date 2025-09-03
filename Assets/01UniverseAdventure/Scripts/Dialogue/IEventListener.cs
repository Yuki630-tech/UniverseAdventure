using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventListener
{
    public string GetId();
    public void Invoke();
}
