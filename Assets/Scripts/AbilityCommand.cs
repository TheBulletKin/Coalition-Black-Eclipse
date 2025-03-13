using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCommand : ICommand
{
	private AbilitySystem abilitySystem;
	private Vector3 targetPosition;
	private int abilityIndex;

	public Coroutine currentCoroutine;

	public event Action<ICommand> OnCommandCompleted;

	public AbilityCommand(AbilitySystem abilitySystem, int abilityIndex, Vector3 targetPosition)
	{
		this.targetPosition = targetPosition;
		this.abilitySystem = abilitySystem;
		this.abilityIndex = abilityIndex;
	}

	public void Cancel(MonoBehaviour executor)
	{
		executor.StopCoroutine(currentCoroutine);
	}

	public void Execute(MonoBehaviour executor)
	{
		abilitySystem.UpdateTargetDetails(null, default, targetPosition);
		abilitySystem.CastAbility(abilityIndex);
		OnCommandCompleted?.Invoke(this);
	}
}
