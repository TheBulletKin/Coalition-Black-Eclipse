using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecoyAbility", menuName = "Abilities/Decoy")]
public class DecoyAbility : CharacterAbility
{
	[SerializeField] private GameObject decoyPrefab;
	public override void Init(AbilitySystem owner)
	{
		base.Init(owner);
	}

	public override void Use(AbilitySystem owner)
	{

	}

	public override void Use(AbilitySystem owner, GameObject target)
	{

	}

	public override void Use(AbilitySystem owner, RaycastHit targetPos)
	{
		GameObject newDecoy = Instantiate(decoyPrefab, targetPos.point, Quaternion.identity);
		EntityManager.Instance.AddNewDecoy(newDecoy.GetComponent<Decoy>());

	}

	public override void Use(AbilitySystem owner, Vector3 targetVecPos)
	{
		if (currentAbilityCount > 0)
		{
			GameObject newDecoy = Instantiate(decoyPrefab, targetVecPos, Quaternion.identity);
			EntityManager.Instance.AddNewDecoy(newDecoy.GetComponent<Decoy>());
			currentAbilityCount--;
		}

	}
}
