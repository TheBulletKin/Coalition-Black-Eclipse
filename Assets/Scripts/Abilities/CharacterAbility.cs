using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterAbility : ScriptableObject
{
	public AbilityTargetType abilityType;
	//When no target
	public abstract void Use(AbilitySystem owner);
	//When targeting a specific gameobject
	public abstract void Use(AbilitySystem owner, GameObject target);
	//When targeting a raycast position
	public abstract void Use(AbilitySystem owner, RaycastHit targetPos);
	//When targeting a precomputed position
	public abstract void Use(AbilitySystem owner, Vector3 targetVecPos);
	public abstract void Init();
}
