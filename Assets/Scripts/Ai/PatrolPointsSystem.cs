using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolPointsSystem : MonoBehaviour
{
	[SerializeField] private PatrolWaypointParent patrolPoints;
	public bool isAtWaypoint = false;
	public float waypointTimer = 0.0f;

	public PatrolWaypoint CurrentPoint => patrolPoints.waypoints[currentPoint];

	private int currentPoint = 0;

	/// <summary>
	/// Gets the next point to patrol to
	/// </summary>
	/// <returns></returns>
	public PatrolWaypoint GetNext()
	{
		PatrolWaypoint patrolWaypoint = patrolPoints.waypoints[currentPoint];
		currentPoint = (currentPoint + 1) % patrolPoints.waypoints.Count;
		return patrolWaypoint;
	}

	private void Update()
	{
		if (isAtWaypoint)
		{
			waypointTimer += Time.deltaTime;
		}

	}

	// Here, making a public method that checks within this class means other classes don't need logic for checking distance


	/// <summary>
	/// Checks if destination reached for any class accessing patrolPoints externally
	/// </summary>
	/// <param name="agent"></param>
	/// <returns></returns>
	public bool HasReached(NavMeshAgent agent)
	{
		if (!agent.pathPending)
		{
			if (agent.remainingDistance <= agent.stoppingDistance)
			{
				if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
				{
					isAtWaypoint = true;
					return true;
				}
			}
		}

		return false;
	}

	public bool WaypointDurationFinished(NavMeshAgent navMeshAgent)
	{
		if (waypointTimer >= CurrentPoint.waitDuration)
		{
			waypointTimer = 0.0f;
			isAtWaypoint = false;
			return true;
		}
		else
		{
			return false;
		}

	}
}
