using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensorObject : MonoBehaviour, IInteractable
{
	
	//Temporarily changed.
	//Prox sensor object has a parent object with a small collider that detects raycasts, for picking it up
	//Child is now just a trigger that ignores raycasts. needed two scripts to handle this behaviour
	public ProxSensorAbility relatedAbility;
	public ProximitySensorTrigger trigger;
	

	public void Interact(GameObject instigator)
	{
		relatedAbility.currentAbilityCount++;

		GameEvents.OnGadgetDestroyed(trigger);
		Destroy(gameObject);
	}
}
