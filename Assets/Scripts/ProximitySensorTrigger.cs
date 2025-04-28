using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensorTrigger : MonoBehaviour, IGadget
{   
	private List<Health> entitiesInProximity = new List<Health>();
	public Transform GadgetTransform => transform;

	//For proximity sensors, the only change that occurs when an enemy is in proximity is a Ui change 

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
				//Setting it to activated will change the ui to show it's activated. Requires no other methods
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
				//Setting it to seactivated will change the ui to show it's activated. Requires no other methods
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
