using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class AbilitySystem : MonoBehaviour, IToggleable
{
	public List<CharacterAbility> abilities;
	public int currentAbilityIndex;
	private Camera playerCamera;
	public LayerMask hittableLayers;
	private GameObject targettedObject;
	private RaycastHit targetPos;
	public bool isPlayerControlled = false;

	private void Start()
	{
		InputManager.Instance.OnUseItemPressed += UseItem;
		InputManager.Instance.OnAbilityChangePressed += SetActiveAbility;

		playerCamera = Camera.main;
		isPlayerControlled = false;

		foreach (CharacterAbility ability in abilities)
		{
			ability.Init();
		}
	}

	private void UseItem()
	{
		if (isPlayerControlled)
		{
			GetUseTarget();
			abilities[currentAbilityIndex].Use(this, targettedObject, targetPos);
		}

	}

	private void GetUseTarget()
	{
		Ray fireRay = playerCamera.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, 20f, hittableLayers))
		{
			targettedObject = hit.collider.gameObject;
			targetPos = hit;
		}
		else
		{
			targettedObject = null;
			targetPos = default;
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

	public void DisableControl()
	{
		isPlayerControlled = false;
	}

	public void EnableControl()
	{
		isPlayerControlled = true;
	}
}
