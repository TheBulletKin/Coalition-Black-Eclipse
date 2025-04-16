using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensorObject : MonoBehaviour, IGadget
{
	public Transform GadgetTransform => transform;
	private List<Health> entitiesInProximity = new List<Health>();

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Enemy"))
		{
			Debug.Log("Enemy entered prox sensor range");

			//Temporary
			Health health = other.GetComponentInParent<Health>();

			if (health != null && !entitiesInProximity.Contains(health))
			{
				health.OnEntityDeath += ClearProxSensor;
				entitiesInProximity.Add(health);
				GameEvents.OnGadgetActivated(this);
				Debug.Log(other.gameObject.name + " entered prox sensor range");
			}		

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

			if (health != null && entitiesInProximity.Contains(health))
			{
				health.OnEntityDeath -= ClearProxSensor;
				entitiesInProximity.Remove(health);
				GameEvents.OnGadgetDeactivated(this);
				Debug.Log(other.gameObject.name + " left prox sensor range");
			}
			
		}
		if (other.gameObject.CompareTag("Player"))
		{
			Debug.Log("Player left prox sensor range");
		}
	}

	private void ClearProxSensor(Health entity)
	{
		foreach (Health health in entitiesInProximity)
		{
			health.OnEntityDeath -= ClearProxSensor;
		}
		
		GameEvents.OnGadgetDeactivated(this);
	}
}
