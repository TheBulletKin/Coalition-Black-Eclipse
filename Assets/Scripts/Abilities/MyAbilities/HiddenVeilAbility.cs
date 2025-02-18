using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HiddenVeilAbility", menuName = "Abilities/Hidden Veil")]
public class HiddenVeilAbility : CharacterAbility
{
	[SerializeField] private int abilityCount = 2;
	[SerializeField] private float placementRange = 10f;
	[SerializeField] private GameObject proxSensorPrefab;
	[SerializeField] private List<HiddenVeil> activeVeils;

	public override void Init()
	{
		abilityCount = 2;
		activeVeils = new List<HiddenVeil>();

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
				activeVeils.Add(newSensor.GetComponent<HiddenVeil>());
			}
		}
	}
}
