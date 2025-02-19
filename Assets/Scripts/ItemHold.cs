using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHold : MonoBehaviour, IInteractable
{
	
	public void Interact(GameObject instigator)
	{
		/* Not a big fan of how this is structured.
		 * It works, but getting components isn't very effective.
		 * 
		 */
		PlayerInteraction interactionSystem = instigator.GetComponent<PlayerInteraction>();
		transform.parent = interactionSystem.holdPoint.transform;
		transform.localPosition = Vector3.zero;
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.isKinematic = true;
		ShootingSystem shootingSystem = instigator.GetComponent<ShootingSystem>();
		shootingSystem.HoldItem(gameObject);
		BottleProjectile bottleProj = GetComponent<BottleProjectile>();
		bottleProj.isThrown = true;
	}
}
