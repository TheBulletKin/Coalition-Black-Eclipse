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
		EnemySightSensor enemySightSensor = stateMachine.GetComponent<EnemySightSensor>();

		//The enemy can die before this action is ran and it changes states in response
		if (enemySightSensor.currentTarget)
		{
			if (!(enemySightSensor.TargetInWeaponRange())) //If target is not currently in weapon range (+ some wiggle room)
			{
				navMeshAgent.isStopped = false;
				navMeshAgent.SetDestination(enemySightSensor.currentTarget.transform.position);
			}
			else
			{
				RaycastHit hit;
				if (!enemySightSensor.TargetInLineOfSight(out hit)) //If within range but without eyes on target
				{
					//Will appear to home in on players. Necessary for the time being
					navMeshAgent.isStopped = false;
					navMeshAgent.SetDestination(enemySightSensor.currentTarget.transform.position);
				}
				else //If in range and line of sight
				{
					
					navMeshAgent.isStopped = true;
					navMeshAgent.ResetPath();
					Vector3 direction = (enemySightSensor.currentTarget.transform.position - stateMachine.transform.position).normalized;
					direction.y = 0;
					stateMachine.transform.rotation = Quaternion.LookRotation(direction);
				}

			}
		}
	}
}
