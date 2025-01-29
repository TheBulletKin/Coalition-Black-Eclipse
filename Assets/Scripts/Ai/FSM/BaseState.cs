using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* An empty state template. Scriptable objects can actually hold methods.
 * 
 */
public class BaseState : ScriptableObject
{
    public virtual void Execute(BaseStateMachine machine)
    {

    }
}
