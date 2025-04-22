using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GrapplePullAbility", menuName = "Abilities/Grapple Pull")]
public class GrapplePullAbility : CharacterAbility, IGadget
{
	public Transform GadgetTransform => null;

	public override void Init(AbilitySystem owner)
	{
		base.Init(owner);
	}



	public override void Use(AbilitySystem owner)
	{

	}

	public override void Use(AbilitySystem owner, GameObject target)
	{
		if (currentAbilityCount > 0)
		{
			if (target != null)
			{
				if (target.CompareTag("Enemy"))
				{
					Vector3 newPosition = owner.GetCastposition() + (owner.GetAimDirection() * 4.0f);

					//Was getting the body hitbox, set it to the parent for the time being
					target.transform.parent.transform.position = newPosition;
					currentAbilityCount--;

					GameEvents.OnGadgetPlaced(this);
				}

			}
		}
	}

	public override void Use(AbilitySystem owner, RaycastHit targetPos)
	{

	}

	public override void Use(AbilitySystem owner, Vector3 targetVecPos)
	{

	}
}
