using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
	private NavMeshAgent agent;

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
	}

	public void MoveTo(Vector3 targetPosition)
	{
		if (agent != null)
		{
			agent.SetDestination(targetPosition);
		}
	}
}
