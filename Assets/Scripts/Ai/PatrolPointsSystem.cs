using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static AIMovement;

public class PatrolPointsSystem : MonoBehaviour
{
	[SerializeField] public PatrolWaypointParent patrolPoints;
	public bool isAtWaypoint = false;
	public float waypointTimer = 0.0f;

	[Header("Temporary attributes")]
	[SerializeField] private float lookRotationSpeed = 2f;


	public PatrolWaypoint currentPoint;

	private int currentPointIndex = 0;

	/// <summary>
	/// Gets the next point to patrol to
	/// </summary>
	/// <returns></returns>
	public PatrolWaypoint SetNextWaypoint()
	{
		currentPointIndex = (currentPointIndex + 1) % patrolPoints.waypoints.Count;
		currentPoint = patrolPoints.waypoints[currentPointIndex];
		return currentPoint;
	}

	private void Start()
	{
		if (patrolPoints.waypoints != null && patrolPoints.waypoints.Count > 0)
		{
			currentPoint = patrolPoints.waypoints[currentPointIndex];
		}


	}


	private void Update()
	{
		if (isAtWaypoint)
		{
			waypointTimer += Time.deltaTime;


			Vector3 worldForward = currentPoint.orientation.transform.TransformDirection(Vector3.forward);


			Quaternion targetRotation = Quaternion.LookRotation(worldForward);

			//transform.rotation = targetRotation;	

			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, lookRotationSpeed * Time.deltaTime);

			transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
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
		if (waypointTimer >= currentPoint.waitDuration)
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
