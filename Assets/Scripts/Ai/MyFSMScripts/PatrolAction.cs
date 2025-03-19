using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Action ran within the state class
[CreateAssetMenu(menuName = "FSM/Actions/Patrol")]
public class PatrolAction : FSMAction
{
	public override void Execute(BaseStateMachine stateMachine)
	{
		//It can ask for the component here, then the state machine will hold it. 
		//So the state machine doesn't necessarily need to hold them all.
		NavMeshAgent navMeshAgent = stateMachine.enemyEntity.navMeshAgent;
		PatrolPointsSystem patrolPoints = stateMachine.enemyEntity.patrolPoints;

		if (patrolPoints.patrolPoints)
		{
			if (patrolPoints.HasReached(navMeshAgent))
			{
				if (patrolPoints.WaypointDurationFinished(navMeshAgent))
				{
					PatrolWaypoint nextWaypoint = patrolPoints.SetNextWaypoint();

					navMeshAgent.SetDestination(nextWaypoint.transform.position);

				}
			}
		}


	}
}
