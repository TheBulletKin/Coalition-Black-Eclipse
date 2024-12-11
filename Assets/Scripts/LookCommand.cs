using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;


public class LookCommand : ICommand
{
	/*
     * Will eventually be more sophisticated, currently will just let the ai look where directed once it reaches this node
     */
	private AIMovement aiEntity;
	private Transform aiTransform;
	private Vector3 targetPosition;
	private NavMeshAgent navAgent;

	

	//Want to change this later so it isn't the command controlling this	
	private float lookRotationDuration = 1f;
	private float lookRotationSpeed = 0.01f;

	public Coroutine currentCoroutine;

	public event Action<ICommand> OnCommandCompleted;

	/// <summary>
	/// Create a LookCommand
	/// </summary>
	/// <param name="aiEntity">The entity that will be looking at something</param>
	/// <param name="targetPosition">The position to look at</param>
	public LookCommand(AIMovement aiEntity, Vector3 targetPosition)
	{
		this.aiTransform = aiEntity.transform;
		this.targetPosition = targetPosition;
		this.navAgent = aiEntity.GetComponent<NavMeshAgent>();
		this.aiEntity = aiEntity;
		

	}

	public void Execute(MonoBehaviour executor)
	{	
	
		
		aiEntity.SetLooking(targetPosition);
		OnCommandCompleted?.Invoke(this);

	}

	

	

	

	public void Cancel(MonoBehaviour executor)
	{
		aiEntity.SetLooking(false);
		navAgent.updateRotation = true;
		executor.StopCoroutine(currentCoroutine);
	}

	
}
