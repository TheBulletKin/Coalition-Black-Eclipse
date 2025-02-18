using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterAbility : ScriptableObject
{
    public abstract void Use(AbilitySystem owner, GameObject target = null, RaycastHit targetPos = default);
	public abstract void Init();
}
