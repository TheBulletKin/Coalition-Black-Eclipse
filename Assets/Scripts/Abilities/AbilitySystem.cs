using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class AbilitySystem : MonoBehaviour, IToggleable, IInitialisable
{

	public List<CharacterAbility> abilities;
	public int currentAbilityIndex;

	[Header("Raycast attributes")]
	[SerializeField] private Camera playerCamera;
	[SerializeField] private LayerMask hittableLayers;

	//Ability targeting values. Updated when an ability is cast to determine cast behaviour
	private GameObject targettedObject;
	private RaycastHit targetPos;
	private Vector3 targetVecPos;

	[Header("Player control")]
	public bool isPlayerControlled = false;
	
	private PlayerCommandIssuer commandIssuer;

	public event Action<int, int> OnAbilitySelected;

	
	/// <summary>
	/// Requires: PlayerCommandIssuer
	/// </summary>
	/// <returns></returns>
	public void Initialize()
	{
		commandIssuer = FindAnyObjectByType<PlayerCommandIssuer>();
		playerCamera = Camera.main;
		isPlayerControlled = false;

		foreach (CharacterAbility ability in abilities)
		{
			ability.Init(this);
		}
	}

	public void CastAbility(int abilityIndex)
	{
		//Use the camera as the raycast position and direction if player controlled
		if (isPlayerControlled && transform != null)
		{
			GetUseTargetDetails(playerCamera.transform.position, playerCamera.transform.forward);
		} else
		{
			if (transform == null)
			{
				return;
			}
			//Otherwise use the Ai's transforms
			GetUseTargetDetails(transform.position, targetVecPos - transform.position);
		}

		if (abilityIndex < abilities.Count)
		{
			//Ability is cast different when on ai to players
			switch (isPlayerControlled ?  abilities[abilityIndex].playerCastAbilityType : abilities[abilityIndex].aiCastAbilityType)
			{
				case AbilityTargetType.TARGET_NO:
					abilities[abilityIndex].Use(this);
					break;
				case AbilityTargetType.GAMEOBJECT_TARGET:
					abilities[abilityIndex].Use(this, targettedObject);
					break;
				case AbilityTargetType.RAYCAST_TARGET:
					abilities[abilityIndex].Use(this, targetPos);
					break;
				case AbilityTargetType.VEC3_TARGET:
					abilities[abilityIndex].Use(this, targetVecPos);
					break;
				default:
					break;
			}
		}


	}

	/// <summary>
	/// Update the ability target details for the next ability cast. Used before player ability cast and before ability command execution
	/// </summary>
	/// <param name="targettedObject">Targetted gameObject</param>
	/// <param name="targetPos">RaycastHit result</param>
	/// <param name="targetVecPos">Vector3 targetted position</param>
	public void UpdateTargetDetails(GameObject targettedObject = null, RaycastHit targetPos = default, Vector3 targetVecPos = default)
	{
		this.targettedObject = targettedObject;
		this.targetPos = targetPos;
		this.targetVecPos = targetVecPos;
	}

	private void PersonalUseItem()
	{
		CastAbility(currentAbilityIndex);
	}

	/// <summary>
	/// Updates targetted gameobject, raycastHit pos and vector3 target position
	/// </summary>
	private void GetUseTargetDetails(Vector3 origin, Vector3 direction)
	{		

		if (Physics.Raycast(origin, direction, out RaycastHit hit, 70f, hittableLayers))
		{			
			UpdateTargetDetails(hit.collider.gameObject, hit, hit.point);
		}
		else
		{			
			UpdateTargetDetails(null, default, default);
		}
	}

	//Performed when the ability change keybind is pressed
	private void SetActiveAbility(int abilityIndex)
	{
		int lastAbilityIndex = currentAbilityIndex;
		currentAbilityIndex = abilityIndex - 2;
		
		if (isPlayerControlled)
		{
			//Ensure the command issuer's ability index matches the ability slot that was just selected
			commandIssuer.currentAbilityIndex = currentAbilityIndex;
			OnAbilitySelected?.Invoke(currentAbilityIndex, lastAbilityIndex);
		}
	}

	public Vector3 GetAimDirection()
	{
		if (isPlayerControlled)
		{
			return playerCamera.transform.forward;
		}
		else
		{
			return transform.forward;
		}
		
	}

	public Vector3 GetCastposition()
	{
		if (isPlayerControlled)
		{
			return playerCamera.transform.position;
		}
		else
		{
			return transform.position;
		}
		
	}

	public void DisableControl()
	{
		isPlayerControlled = false;
		InputManager.Instance.OnUseItemPressed -= PersonalUseItem;
		InputManager.Instance.OnAbilityChangePressed -= SetActiveAbility;
	}

	public void EnableControl()
	{
		isPlayerControlled = true;
		SetActiveAbility(2);
		InputManager.Instance.OnUseItemPressed += PersonalUseItem;
		InputManager.Instance.OnAbilityChangePressed += SetActiveAbility;
	}
}
