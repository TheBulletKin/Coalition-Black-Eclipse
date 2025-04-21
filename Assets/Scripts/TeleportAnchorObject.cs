using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportAnchorObject : MonoBehaviour, IGadget, IInteractable
{
	public Transform GadgetTransform => transform;
	public TeleportAnchor relatedAbility;

	public void Interact(GameObject instigator)
	{
		relatedAbility.currentAbilityCount = 1;
		relatedAbility.anchorActive = false;
		relatedAbility.teleportPosition = default;

		GameEvents.OnGadgetDestroyed(this);
		Destroy(gameObject);
	}
}
