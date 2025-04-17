using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[CreateAssetMenu(fileName = "SubterfugeAbility", menuName = "Abilities/Subterfuge")]
public class SubterfugeAbility : CharacterAbility, IGadget
{
	//Temporary while better solution found

	public bool IsDisguised => isDisguised;
	private bool isDisguised = false;

	public int availableUses = 1;
	public int abilityUses = 1;

	public Transform GadgetTransform => affectedEntity.transform;

	public EntityVisibility affectedEntity;

	public float disguiseDuration;

	public override void Init(AbilitySystem owner)
	{
		isDisguised = false;
		availableUses = abilityUses;
		affectedEntity = null;
	}


	public override void Use(AbilitySystem owner)
	{
		/* Temporary setup to get this working
		 * Will get the entity visibility component and start a duration on that
		 */

		if (affectedEntity == null)
		{
			affectedEntity = owner.gameObject.GetComponent<EntityVisibility>();
		}


		if (affectedEntity == null)
		{
			Debug.LogWarning("SubterfugeAbility: Affected entity is null");
			return;
		}


		if (availableUses <= 0)
		{
			Debug.LogWarning("SubterfugeAbility: All disguise abilities used up");
			return;
		}

		if (affectedEntity.IsHidden())
		{
			Debug.LogWarning(affectedEntity.name + " is already disguised");
			return;
		}

		TriggerDisguise(owner);

	}

	public override void Use(AbilitySystem owner, GameObject target)
	{

	}

	public override void Use(AbilitySystem owner, RaycastHit targetPos)
	{

	}

	public override void Use(AbilitySystem owner, Vector3 targetVecPos)
	{

	}

	public void OnDisguiseDurationFinished()
	{
		GameEvents.OnGadgetDeactivated(this);
		GameEvents.OnGadgetDestroyed(this);
	}

	private void TriggerDisguise(AbilitySystem owner)
	{
		affectedEntity = owner.gameObject.GetComponent<EntityVisibility>();

		if (affectedEntity != null)
		{
			isDisguised = !isDisguised;
			float newVis = isDisguised ? 0.0f : 1.0f;

			affectedEntity.HideForDuration(disguiseDuration, 0);
			affectedEntity.SetDurationCompletionCallback(OnDisguiseDurationFinished);

			//broadcast so ui gadget manager can create Ui elements
			GameEvents.OnGadgetPlaced(this);
			GameEvents.OnGadgetActivated(this);

			availableUses -= 1;
		}
		else
		{
			Debug.LogWarning("Subterfuge Ability : No EntityVisibility component found on ability owner");
		}
	}







}
