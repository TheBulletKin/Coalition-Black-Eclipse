using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/Has Killed Target")]
public class HasKilledTargetDecision : Decision
{
	//Ran while in the engage state
	public override bool Decide(BaseStateMachine stateMachine)
	{
		EnemySightSensor sightSensor = stateMachine.GetComponent<EnemySightSensor>();
		if (sightSensor.currentTarget == null) //Means the target is dead
		{			
			return true;
		}

		return false;
	}
}
