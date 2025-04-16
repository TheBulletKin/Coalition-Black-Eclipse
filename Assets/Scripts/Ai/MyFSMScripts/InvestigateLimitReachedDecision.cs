using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/Investigation Limit Reached")]
public class InvestigateLimitReachedDecision : Decision
{
	public override bool Decide(BaseStateMachine stateMachine)
	{
		AiSoundSensor soundSensor = stateMachine.aiBrain.soundSensor;
		if (soundSensor.timeSinceSoundHeard >= soundSensor.investigateTimeLimit)
		{
			soundSensor.ForgetSound();
			return true;
		}

		return false;
	}
}
