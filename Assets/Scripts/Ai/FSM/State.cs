using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* To implement a data diven approach, actions and transitions have been abstracted out and simplified.
 * Will simply execute each action then check transitions to see if it should change state.
 */
[CreateAssetMenu(menuName = "FSM/State")]
public class State : BaseState
{
	public List<FSMAction> Action = new List<FSMAction>();
	public List<Transition> Transitions = new List<Transition>();

	public override void Execute(BaseStateMachine machine)
	{
		//Actions are perfomed based on the state. patrol actions involve moving from waypoint to waypoint, for instance
		foreach (var action in Action)
			action.Execute(machine);

		//Transitions run after the action to check whether the state should change
		foreach (var transition in Transitions)
			transition.Execute(machine);
	}

}
