using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrapplePullAbility", menuName = "Abilities/Grapple Pull")]
public class GrapplePullAbility : CharacterAbility
{
	public override void Init()
	{

	}

	public override void Use(AbilitySystem owner, GameObject target, RaycastHit targetPos = default)
	{
		if (target != null)
		{
			if (target.CompareTag("Enemy"))
				target.transform.position = owner.GetCastposition() + (owner.GetAimDirection() * 2.0f);
		}

	}
}
