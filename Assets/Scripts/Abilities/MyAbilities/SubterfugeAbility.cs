using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[CreateAssetMenu(fileName = "SubterfugeAbility", menuName = "Abilities/Subterfuge")]
public class SubterfugeAbility : CharacterAbility
{
	private bool isDisguised = false;
	public override void Init()
	{
		isDisguised = false;

	}

	public override void Use(AbilitySystem owner, GameObject target = null)
	{

		EntityVisibility entityVis = owner.gameObject.GetComponent<EntityVisibility>();
		if (entityVis != null)
		{
			isDisguised = !isDisguised;
			float newVis = isDisguised ? 0.0f : 1.0f;
			entityVis.ChangeVisibilityModifier(newVis);
		}
		else
		{
			Debug.LogWarning("Subterfuge Ability : No EntityVisibility component found on ability owner");
		}

	}
}
