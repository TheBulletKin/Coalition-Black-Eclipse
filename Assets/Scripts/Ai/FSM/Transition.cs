using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Pass in a decision as a scriptable object, and two states.
 * Runs a test using that decision method to see if it should swap states
 * Using a separate RemainInState class that is itself a scriptable object just tells it to not change class
 * Abstracts the decisionmaking process and stays data driven, whether it switches or not is based on the inspector
 */
[CreateAssetMenu(menuName = "FSM/Transition")]
public sealed class Transition : ScriptableObject
{
	public Decision Decision;
	public BaseState TrueState;
	public BaseState FalseState;

	public void Execute(BaseStateMachine stateMachine)
	{
		if (Decision.Decide(stateMachine) && !(TrueState is RemainInState))
		{
			stateMachine.CurrentState = TrueState;
			Debug.Log(stateMachine.gameObject.name + " now entering " + TrueState.name + " state");
		}			
		else if (!(FalseState is RemainInState))
		{
			stateMachine.CurrentState = FalseState;
		}
			
	}
}
