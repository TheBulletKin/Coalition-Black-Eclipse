using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MoveCommand : ICommand
{
	private AIMovement aiMovement;
	private Vector3 targetPosition;
	private NavMeshAgent navAgent;

	public Coroutine currentCoroutine;

	//Interface events
	public event Action<ICommand> OnCommandCompleted;

	/// <summary>
	/// A move command object
	/// </summary>
	/// <param name="aiMovement">The ai agent being told to move</param>
	/// <param name="targetPosition">The location to move to</param>
	public MoveCommand(AIMovement aiMovement, Vector3 targetPosition)
	{
		this.aiMovement = aiMovement;
		this.targetPosition = targetPosition;
		this.navAgent = aiMovement.GetComponent<NavMeshAgent>();
	}	

	/// <summary>
	/// Interface method of ICommand
	/// </summary>
	public void Execute(MonoBehaviour executor)
	{
		currentCoroutine = executor.StartCoroutine(FullMoveTo());
	}

	/// <summary>
	/// Coroutine to perform agent movement, asynchronous to allow for other tasks to perform at the same time
	/// </summary>
	/// <returns></returns>
	private IEnumerator FullMoveTo()
	{
		//Start moving to destination
		aiMovement.MoveTo(targetPosition);

		//Check whether the destination has been reached, wait till the next frame then try again
		while (navAgent.pathPending || navAgent.remainingDistance > navAgent.stoppingDistance)
		{
			yield return null;
		}		

		//Invoke the completed event which is read by the command issuer to perform tasks on completion		 
		OnCommandCompleted?.Invoke(this);
	}

	public void Cancel(MonoBehaviour executor)
	{
		executor.StopCoroutine(currentCoroutine);
	}
}
