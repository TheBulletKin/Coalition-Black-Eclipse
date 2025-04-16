using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "FSM/Actions/Investigate")]
public class InvestigateAction : FSMAction
{
	//Distance from sound to stop
	public float destinationThreshold = 3.0f;
	
	public override void Execute(BaseStateMachine stateMachine)
	{

		NavMeshAgent navMeshAgent = stateMachine.enemyEntity.navMeshAgent;
		AiSoundSensor soundSensor = stateMachine.aiBrain.soundSensor;
		AIMovement aiMovement = stateMachine.aiBrain.aiMovement;

		if (soundSensor != null)
		{
			Vector3 targetPosition = soundSensor.soundHeard.soundPos;

			aiMovement.SetLooking(targetPosition);

			float distanceToTarget = Vector3.Distance(navMeshAgent.transform.position, targetPosition);

			if (distanceToTarget > destinationThreshold)
			{
				aiMovement.MoveTo(targetPosition);
			}
			else if (distanceToTarget <= destinationThreshold)
			{
				navMeshAgent.ResetPath();
			}


		}
			
	}
}
