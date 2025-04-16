using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Uses the sound sensor to change state when a sound has been heard
[CreateAssetMenu(menuName = "FSM/Decisions/Has Heard Sound")]
public class HasHeardSoundDecision : Decision
{
	public override bool Decide(BaseStateMachine stateMachine)
	{
		AiSoundSensor soundSensor = stateMachine.aiBrain.soundSensor;
		if (soundSensor.hasHeardSound)
		{
			return true;
		}

		return false;
	}
}
