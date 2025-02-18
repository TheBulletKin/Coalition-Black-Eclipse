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

	public override void Use(AbilitySystem owner, GameObject target = null, RaycastHit targetPos = default)
	{
		if (targetPos.collider == null)
		{
			return;
		}
		else
		{
			if (Vector3.Distance(owner.transform.position, targetPos.point) <= placementRange)
			{
				GameObject newSensor = Instantiate(proxSensorPrefab, targetPos.point, Quaternion.FromToRotation(Vector3.up, targetPos.normal));
				activeSensors.Add(newSensor.GetComponent<ProximitySensorObject>());
			}
		}
	}
}
