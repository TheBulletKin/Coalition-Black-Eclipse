using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
	private NavMeshAgent agent;

	[Serializable]
	public class RotationAndLookConfig
	{

		public float lookRotationSpeed = 0.01f;
	}

	[SerializeField] private RotationAndLookConfig rotationAndLookConfig;

	public Vector3 lookTarget;
	public bool isPieingTarget = false;
	public float lookRotationDuration = 1f;

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

	public void SetLooking(bool state)
	{
		isPieingTarget = state;
		if (state == true)
		{
			agent.updateRotation = false;
		}
		else
		{
			agent.updateRotation = true;
		}
	}
	
	public void SetLooking(Vector3 targetPosition)
	{
		lookTarget = targetPosition;
		isPieingTarget = true;
		agent.updateRotation = false;
	}

	private void Update()
	{
		if (isPieingTarget)
		{
			Vector3 lookDirection = (lookTarget - gameObject.transform.position).normalized;			
			Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationAndLookConfig.lookRotationSpeed * Time.deltaTime);

			transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
		}
	}


}
