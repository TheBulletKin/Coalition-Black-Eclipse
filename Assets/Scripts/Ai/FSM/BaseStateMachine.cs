using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Everything stems from this base state machine
 * Needs only know about the state that is currently active, and execute that state alone.
 * Creating a method with new will override unity's base implementation.
 * 
 */
public class BaseStateMachine : MonoBehaviour
{
	[SerializeField] private BaseState initialState;

	public BaseState CurrentState { get; set; }

	[Header("Entity components")]
	public EnemyEntity enemyEntity;
	public AiBrain aiBrain;		

	private void Awake()
	{
		CurrentState = initialState;		
		enemyEntity = GetComponent<EnemyEntity>();
		aiBrain = GetComponent<AiBrain>();
	}

	private void Update()
	{
		//Can slot in any state and have it execute it's own methods
		CurrentState.Execute(this);
	}
}
