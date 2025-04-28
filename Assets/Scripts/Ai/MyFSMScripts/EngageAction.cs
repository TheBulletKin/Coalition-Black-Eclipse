using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Action ran within the state class
[CreateAssetMenu(menuName = "FSM/Actions/Engage")]
public class EngageAction : FSMAction
{
	//Actions for the engage state. Boils down to chasing and looking at the targetted enemy.


	//Since SOs are shared, independant data cannot be held so it will currently update all info per ai
	//Not particularly good performance wise as it's constantly swapping values, will consider another approach later when
	//   abilities are tackled also
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
