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

		if (Vector3.Distance(owner.transform.position, targetVecPos) <= placementRange && abilityCount > 0)
		{
			GameObject newVeil = Instantiate(proxSensorPrefab, targetVecPos, Quaternion.identity);
			activeVeils.Add(newVeil.GetComponent<HiddenVeil>());
		}

	}
}
