using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Action ran within the state class
[CreateAssetMenu(menuName = "FSM/Actions/Engage")]
public class EngageAction : FSMAction
{
	
	//Getting the components dynamically means that nothing is stored in the SO. Badly optimised however
	//If something was stored here, each ai would share state information. Will need a fix for this later
	public override void Execute(BaseStateMachine stateMachine)
	{
		NavMeshAgent navMeshAgent = stateMachine.GetComponent<NavMeshAgent>();
		AiSightSensor sightSensor = stateMachine.aiBrain.sightSensor;
		AiSoundSensor soundSensor = stateMachine.aiBrain.soundSensor;
		AIMovement aiMovement = stateMachine.aiBrain.aiMovement;

		soundSensor.hasHeardSound = false;

		if (sightSensor.currentTarget)
		{
			Transform engageTarget = sightSensor.currentTarget.transform;
			//The enemy can die before this action is ran and it changes states in response

			if (engageTarget == null)
			{
				engageTarget = sightSensor.currentTarget.transform;
			}
			if (!(sightSensor.TargetInWeaponRange())) //Isn't in weapon range
			{
				aiMovement.MoveTo(engageTarget.position);
				aiMovement.SetLooking(false);
			}
			else //Is in weapon range
			{
				RaycastHit hit;
				if (!sightSensor.TargetInLineOfSight(sightSensor.currentTarget, out hit)) //Isn't within line of sight
				{
					aiMovement.MoveTo(engageTarget.position);
					aiMovement.SetLooking(false);
				}
				else //Is within line of sight
				{
					/* Optional if I want the ai to not rotate when in the vision cone
					if (sightSensor.TargetInVisionCone(engageTarget)) //Is in vision cone
					{
						if (sightSensor.TargetInPreferredVisionCone(engageTarget))
						{
							aiMovement.SetLooking(false);
						}						
					}
					else
					{
						aiMovement.SetLooking(engageTarget);
					}
					*/

					navMeshAgent.ResetPath();

					aiMovement.SetLooking(engageTarget.position);
				}
			}
		}
		

	}
}
