using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Template class for an action for a state, SO so it is data driven. Inherit and create custom implementation
public abstract class FSMAction : ScriptableObject
{
   public abstract void Execute(BaseStateMachine stateMachine);
}
