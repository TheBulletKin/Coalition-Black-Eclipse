using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolPoints : MonoBehaviour
{
	[SerializeField] private Transform[] patrolPoints;

	public Transform CurrentPoint => patrolPoints[currentPoint];

	private int currentPoint = 0;

	/// <summary>
	/// Gets the next point to patrol to
	/// </summary>
	/// <returns></returns>
	public Transform GetNext()
	{
		var point = patrolPoints[currentPoint];
		currentPoint = (currentPoint + 1) % patrolPoints.Length;
		return point;
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
					return true;
				}
			}
		}

		return false;
	}
}
