using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportAnchorObject : MonoBehaviour, IGadget, IInteractable
{
	public Transform GadgetTransform => transform;	

	private event Action OnAnchorPickup;

	public void Interact(GameObject instigator)
	{
		OnAnchorPickup?.Invoke();
	}

	public void SetPickupCallback(Action callback)
	{
		OnAnchorPickup = callback;
	}
}
