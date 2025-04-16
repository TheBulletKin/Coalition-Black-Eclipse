using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensorObject : MonoBehaviour, IGadget
{
	public Transform GadgetTransform => transform;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Enemy"))
		{
			Debug.Log("Enemy entered prox sensor range");

			//Temporary
			Health health = other.GetComponentInParent<Health>();
			health.OnEntityDeath += ClearProxSensor;

			GameEvents.OnGadgetActivated(this);
		}
		if (other.gameObject.CompareTag("Player"))
		{
			Debug.Log("Player entered prox sensor range");
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Enemy"))
		{
			Debug.Log("Enemy left prox sensor range");

			Health health = other.GetComponentInParent<Health>();
			health.OnEntityDeath -= ClearProxSensor;

			GameEvents.OnGadgetDeactivated(this);
		}
		if (other.gameObject.CompareTag("Player"))
		{
			Debug.Log("Player left prox sensor range");
		}
	}

	private void ClearProxSensor(Health entity)
	{
		entity.OnEntityDeath -= ClearProxSensor;
		GameEvents.OnGadgetDeactivated(this);
	}
}
