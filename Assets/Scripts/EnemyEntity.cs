using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyEntity : MonoBehaviour, IInteractable
{
	[SerializeField] public bool isRevealed = false;
	public BaseStateMachine stateMachine;
	public PatrolPointsSystem patrolPoints;
	public NavMeshAgent navMeshAgent;

	private void Start()
	{
		stateMachine = GetComponent<BaseStateMachine>();
		patrolPoints = GetComponent<PatrolPointsSystem>();
		navMeshAgent = GetComponent<NavMeshAgent>();
	}

	public void Interact(GameObject instigator)
	{
		isRevealed = true;
	}
}
