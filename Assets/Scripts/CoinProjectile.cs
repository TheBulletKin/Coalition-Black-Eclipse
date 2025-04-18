using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinProjectile : MonoBehaviour, IGadget
{
	private Action<Vector3, Vector3> onLandedCallback;

	public Transform GadgetTransform => transform;

	public void SetAnchorCallback(Action<Vector3, Vector3> callback)
	{
		onLandedCallback = callback;
	}



	private void OnCollisionEnter(Collision collision)
	{
		ContactPoint contact = collision.contacts[0];
		onLandedCallback?.Invoke(contact.point, contact.normal);

		GameEvents.OnGadgetPlaced?.Invoke(this);

		Destroy(gameObject);
	}
}
