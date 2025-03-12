using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Uses the sight sensor to detect enemies and return a value
[CreateAssetMenu(menuName = "FSM/Decisions/In Line Of Sight")]
public class InLineOfSightAndDetectedDecision : Decision
{
	//Getting the components dynamically means that nothing is stored in the SO
	//If something was stored here, each ai would share state information. Will need a fix for this later
	public override bool Decide(BaseStateMachine stateMachine)
	{
		EnemySightSensor sightSensor = stateMachine.GetComponent<EnemySightSensor>();
		
		return sightSensor.entityIsDetected;
	}
}
