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
		

	}

	public void Execute(MonoBehaviour executor)
	{
		currentCoroutine = executor.StartCoroutine(FullLookAt());
	}
	
	public IEnumerator FullLookAt()
	{
		Vector3 lookDirection = (targetPosition - aiTransform.position).normalized;
		Quaternion startRotation = aiTransform.rotation;
		Quaternion targetRotation = Quaternion.LookRotation(lookDirection);	
		

		float elapsedTime = 0f;		
		
		while (elapsedTime < lookRotationDuration)
		{

			float progress = elapsedTime / lookRotationDuration;


			aiTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, progress);
			aiTransform.rotation = Quaternion.Euler(0, aiTransform.rotation.eulerAngles.y, 0);

			elapsedTime += Time.deltaTime;

			yield return null;
		}

		aiTransform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
		OnCommandCompleted?.Invoke(this);

	}

	public void Cancel(MonoBehaviour executor)
	{
		executor.StopCoroutine(currentCoroutine);
	}

	
}
