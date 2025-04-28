using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[CreateAssetMenu(fileName = "SubterfugeAbility", menuName = "Abilities/Subterfuge")]
public class SubterfugeAbility : CharacterAbility, IGadget
{	
	private bool isDisguised = false;

	public Transform GadgetTransform => entityVisibility.transform;

	
	public EntityVisibility entityVisibility;
	public ControllableEntity entity;

	public float disguiseDuration;

	public override void Init(AbilitySystem owner)
	{
		base.Init(owner);
		isDisguised = false;		
		entityVisibility = null;
		entity = null;
	}


	public override void Use(AbilitySystem owner)
	{

		if (entityVisibility == null)
		{
			//Currently, since abilities are held on scriptableObjects, these values are shared.
			//So only one character can have this ability for now.
			//Will therefore assign the character to affect when first used and tie the ability to them.
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

		if (currentAbilityCount <= 0)
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
		//Used to update the UI when the duration is complete.
		GameEvents.OnGadgetDeactivated(this);
		GameEvents.OnGadgetDestroyed(this);
	}

	private void TriggerDisguise(AbilitySystem owner)
	{
		//Created before the introduction of the hidden status effect. Will change it soon to fit with that system.
		//At the moment it will just directly use the visibility component
		entityVisibility = owner.gameObject.GetComponent<EntityVisibility>();

		if (entityVisibility != null)
		{
			isDisguised = !isDisguised;
			float newVis = isDisguised ? 0.0f : 1.0f;

			entityVisibility.HideForDuration(disguiseDuration, 0);
			entityVisibility.SetDurationCompletionCallback(OnDisguiseDurationFinished);

			currentAbilityCount--;

			//broadcast so ui gadget manager can create Ui elements
			GameEvents.OnGadgetPlaced(this);
			GameEvents.OnGadgetActivated(this);			
		}
		else
		{
			Debug.LogWarning("Subterfuge Ability : No EntityVisibility component found on ability owner");
		}
	}







}
