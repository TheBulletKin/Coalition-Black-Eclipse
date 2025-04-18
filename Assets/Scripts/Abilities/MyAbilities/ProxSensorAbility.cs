using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "ProxSensorAbility", menuName = "Abilities/Prox Sensor")]
public class ProxSensorAbility : CharacterAbility
{   
	[SerializeField] private float placementRange = 5f;
	[SerializeField] private float launchForce = 20f;
	[SerializeField] private GameObject sensorObjectPrefab;
	[SerializeField] private GameObject sensorProjectilePrefab;
	[SerializeField] private List<ProximitySensorObject> activeSensors;

	public override void Init(AbilitySystem owner)
	{
		base.Init(owner);
		activeSensors = new List<ProximitySensorObject>();
		
	}	

	public override void Use(AbilitySystem owner)
	{
		if (currentAbilityCount > 0)
		{

			GameObject projectile = Instantiate(sensorProjectilePrefab, owner.GetCastposition() + owner.GetAimDirection() * 1.05f, Quaternion.identity);

			Rigidbody rb = projectile.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.velocity = owner.GetAimDirection() * launchForce;
			}

			ProximitySensorProjectile projectileScript = projectile.GetComponent<ProximitySensorProjectile>();
			if (projectileScript != null)
			{
				projectileScript.SetSensorCallback(CreateProximitySensor);
			}
			currentAbilityCount--;
		}
	}

	public override void Use(AbilitySystem owner, GameObject target)
	{
		
	}

	public override void Use(AbilitySystem owner, RaycastHit targetPos)
	{

	}

	public override void Use(AbilitySystem owner, Vector3 targetVecPos)
	{
		if (Vector3.Distance(owner.transform.position, targetVecPos) <= placementRange && currentAbilityCount > 0)
		{		
			CreateProximitySensor(targetVecPos, Vector3.up);
		}
	}

	private void CreateProximitySensor(Vector3 position, Vector3 normal)
	{
		if (sensorObjectPrefab != null)
		{
			GameObject proximitySensor = Instantiate(sensorObjectPrefab, position, Quaternion.LookRotation(-normal, Vector3.up));
			proximitySensor.transform.up = normal;
			ProximitySensorObject sensorObject = proximitySensor.GetComponent<ProximitySensorObject>();
			activeSensors.Add(sensorObject);

			GameEvents.OnGadgetPlaced?.Invoke(sensorObject);
		}
	}
}
