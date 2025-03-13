using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "ProxSensorAbility", menuName = "Abilities/Prox Sensor")]
public class ProxSensorAbility : CharacterAbility
{
    [SerializeField] private int gadgetCount = 2;
	[SerializeField] private float placementRange = 5f;
	[SerializeField] private GameObject proxSensorPrefab;
	[SerializeField] private List<ProximitySensorObject> activeSensors;

	public override void Init()
	{
		gadgetCount = 2;
		activeSensors = new List<ProximitySensorObject>();
		
	}

	

	public override void Use(AbilitySystem owner)
	{
	
	}

	public override void Use(AbilitySystem owner, GameObject target)
	{
		
	}

	public override void Use(AbilitySystem owner, RaycastHit targetPos)
	{

	}

	public override void Use(AbilitySystem owner, Vector3 targetVecPos)
	{
		if (Vector3.Distance(owner.transform.position, targetVecPos) <= placementRange)
		{
			GameObject newSensor = Instantiate(proxSensorPrefab, targetVecPos, Quaternion.identity);
			activeSensors.Add(newSensor.GetComponent<ProximitySensorObject>());
		}
	}
}
