using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class AbilitySystem : MonoBehaviour
{
    public List<CharacterAbility> abilities;
    public int currentAbilityIndex;
	public Camera playerCamera;

	private void Start()
	{
		InputManager.Instance.OnUseItemPressed += UseItem;
		InputManager.Instance.OnAbilityChangePressed += SetActiveAbility;

		foreach (CharacterAbility ability in abilities)
		{
			ability.Init();
		}
	}

	private void UseItem()
	{
		abilities[currentAbilityIndex].Use(this);
	}

	private void SetActiveAbility(int abilityIndex)
	{
		currentAbilityIndex = abilityIndex - 2;
	}

	public Vector3 GetAimDirection()
	{
		return playerCamera.transform.forward;
	}

	public Vector3 GetCastposition()
	{
		return playerCamera.transform.position;
	}
}
