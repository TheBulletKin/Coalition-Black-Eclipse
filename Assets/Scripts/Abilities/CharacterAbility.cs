using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class CharacterAbility : ScriptableObject
{
	public AbilityTargetType aiCastAbilityType;
	public AbilityTargetType playerCastAbilityType;
	public Image abilityIcon;
	public string abilityName;
	public bool hasCountLimit;
	public int countLimit;
	public int currentAbilityCount;	

	//When no target
	public abstract void Use(AbilitySystem owner);
	//When targeting a specific gameobject
	public abstract void Use(AbilitySystem owner, GameObject target);
	//When targeting a raycast position
	public abstract void Use(AbilitySystem owner, RaycastHit targetPos);
	//When targeting a precomputed position
	public abstract void Use(AbilitySystem owner, Vector3 targetVecPos);
	public virtual void Init(AbilitySystem owner)
	{
		currentAbilityCount = countLimit;
	}	
}
