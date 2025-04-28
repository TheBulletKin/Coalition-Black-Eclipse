using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensorProjectile : MonoBehaviour
{
	
	private Action<Vector3, Vector3> onLandedCallback;

	public void SetSensorCallback(Action<Vector3, Vector3> callback)
	{
		//Handle the proximity sensor creation script on the ability script itself instead of the projectile
		onLandedCallback = callback;
	}

	private void Start()
	{		
		Destroy(gameObject, 6.0f);
	}

	private void OnCollisionEnter(Collision collision)
	{
		ContactPoint contact = collision.contacts[0];
		onLandedCallback?.Invoke(contact.point, contact.normal);		
		
		Destroy(gameObject);
	}
}
