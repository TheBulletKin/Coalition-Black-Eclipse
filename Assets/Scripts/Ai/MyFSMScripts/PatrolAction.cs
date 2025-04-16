using System.Collections;
using System.Collections.Generic;
using TMPro;
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
		AIMovement aiMovement = stateMachine.aiBrain.aiMovement;
		aiMovement.SetLooking(false);

		if (patrolPoints.patrolPoints != null && patrolPoints.patrolPoints.waypoints.Count > 0)
		{
			if (patrolPoints.HasReached(navMeshAgent))
			{
				if (patrolPoints.WaypointDurationFinished(navMeshAgent))
				{
					PatrolWaypoint nextWaypoint = patrolPoints.SetNextWaypoint();				
					aiMovement.MoveTo(nextWaypoint.transform.position);
				}
			}
		}
		else
		{
			if (Vector3.Distance(navMeshAgent.destination, patrolPoints.initialPosition) > 0.1 && !navMeshAgent.pathPending)
			{
				aiMovement.MoveTo(patrolPoints.initialPosition);
			}		
		}


	}
}
