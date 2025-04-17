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

	public Transform GadgetTransform => entityVisibility.transform;

	public EntityVisibility entityVisibility;
	public ControllableEntity entity;

	public float disguiseDuration;

	public override void Init(AbilitySystem owner)
	{
		isDisguised = false;
		availableUses = abilityUses;
		entityVisibility = null;
		entity = null;
	}


	public override void Use(AbilitySystem owner)
	{
		/* Temporary setup to get this working
		 * Will get the entity visibility component and start a duration on that
		 */

		if (entityVisibility == null)
		{
			entityVisibility = owner.gameObject.GetComponent<EntityVisibility>();
		}

		if (entity == null)
		{
			entity = owner.gameObject.GetComponent<ControllableEntity>();
		}


		if (entityVisibility == null)
		{
			Debug.LogWarning("SubterfugeAbility: Affected entity is null");
			return;
		}


		if (availableUses <= 0)
		{
			Debug.LogWarning("SubterfugeAbility: All disguise abilities used up");
			return;
		}

		if (entityVisibility.IsHidden())
		{
			Debug.LogWarning(entityVisibility.name + " is already disguised");
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
		entityVisibility = owner.gameObject.GetComponent<EntityVisibility>();

		if (entityVisibility != null)
		{
			isDisguised = !isDisguised;
			float newVis = isDisguised ? 0.0f : 1.0f;

			entityVisibility.HideForDuration(disguiseDuration, 0);
			entityVisibility.SetDurationCompletionCallback(OnDisguiseDurationFinished);

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
