using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterAbility : ScriptableObject
{
    public abstract void Use(AbilitySystem owner, GameObject target = null, Vector3 targetPos = default);
	public abstract void Init();
}
