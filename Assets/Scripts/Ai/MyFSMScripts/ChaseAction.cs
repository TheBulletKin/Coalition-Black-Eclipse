using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Action ran within the state class
[CreateAssetMenu(menuName = "FSM/Actions/Chase")]
public class ChaseAction : FSMAction
{
	//Getting the components dynamically means that nothing is stored in the SO. Badly optimised however
	//If something was stored here, each ai would share state information. Will need a fix for this later
	public override void Execute(BaseStateMachine stateMachine)
	{
		NavMeshAgent navMeshAgent = stateMachine.GetComponent<NavMeshAgent>();
		EnemySightSensor enemySightSensor = stateMachine.GetComponent<EnemySightSensor>();

		navMeshAgent.SetDestination(enemySightSensor.currentTarget.position);
	}
}
