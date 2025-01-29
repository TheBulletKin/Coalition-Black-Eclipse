using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Template scriptable object to create custom implementation for
public abstract class Decision : ScriptableObject
{
    public abstract bool Decide(BaseStateMachine stateMachine);
}
