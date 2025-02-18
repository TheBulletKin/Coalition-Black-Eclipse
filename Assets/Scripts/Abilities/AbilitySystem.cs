using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class AbilitySystem : MonoBehaviour
{
    public List<CharacterAbility> abilities;
    public int currentAbilityIndex;
	private Camera playerCamera;
	public LayerMask hittableLayers;

	private void Start()
	{
		InputManager.Instance.OnUseItemPressed += UseItem;
		InputManager.Instance.OnAbilityChangePressed += SetActiveAbility;

		playerCamera = Camera.main;

		foreach (CharacterAbility ability in abilities)
		{
			ability.Init();
		}
	}

	private void UseItem()
	{
		abilities[currentAbilityIndex].Use(this, GetUseTarget());
	}

	private GameObject GetUseTarget()
	{
		Ray fireRay = playerCamera.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 20f, hittableLayers))
		{			
			return hit.collider.gameObject;
		}
		else
		{
			return null;
		}
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
