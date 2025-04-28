using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensorObject : MonoBehaviour, IInteractable
{	
	//Sensor object has two parts, a small collider (this script) which allows for picking up on interaction
	//Second part is a larger collider used to handle entering and exiting
	
	public ProximitySensorTrigger trigger;

	private event Action<ProximitySensorObject> OnSensorPickup;

	public void Interact(GameObject instigator)
	{
		OnSensorPickup?.Invoke(this);
	}

	public void SetPickupCallback(Action<ProximitySensorObject> callback)
	{
		OnSensorPickup = callback;
	}
}
