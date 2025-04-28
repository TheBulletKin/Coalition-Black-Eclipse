using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class CharacterAbility : ScriptableObject
{
	[Header("Ability Attributes")]
	public AbilityTargetType aiCastAbilityType;
	public AbilityTargetType playerCastAbilityType;
	public Image abilityIcon;
	public string abilityName;
	public bool hasCountLimit;
	public int countLimit;
	public int currentAbilityCount;	

	/// <summary>
	/// Used when the ability requires no target (thrown projectile etc)
	/// </summary>
	/// <param name="owner"></param>
	public abstract void Use(AbilitySystem owner);
	/// <summary>
	/// When the ability is cast on a selected gameobject (status effects etc)
	/// </summary>
	/// <param name="owner"></param>
	/// <param name="target"></param>
	public abstract void Use(AbilitySystem owner, GameObject target);
	/// <summary>
	/// When the ability cast requires a raycast to confirm placement
	/// </summary>
	/// <param name="owner"></param>
	/// <param name="targetPos"></param>
	public abstract void Use(AbilitySystem owner, RaycastHit targetPos);
	/// <summary>
	/// When the ability requires a targetted vec3 (placed gadgets etc)
	/// </summary>
	/// <param name="owner"></param>
	/// <param name="targetVecPos"></param>
	public abstract void Use(AbilitySystem owner, Vector3 targetVecPos);

	/// <summary>
	/// Set up ability counts
	/// </summary>
	/// <param name="owner"></param>
	public virtual void Init(AbilitySystem owner)
	{
		currentAbilityCount = countLimit;
	}	
}
