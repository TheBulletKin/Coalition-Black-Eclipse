using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecoyAbility", menuName = "Abilities/Decoy")]
public class DecoyAbility : CharacterAbility
{
	[SerializeField] private GameObject decoyPrefab;
	public override void Init()
	{
		
	}	

	public override void Use(AbilitySystem owner)
	{
		
	}

	public override void Use(AbilitySystem owner, GameObject target)
	{
		
	}

	public override void Use(AbilitySystem owner, RaycastHit targetPos)
	{
		Instantiate(decoyPrefab, targetPos.point, Quaternion.identity);
	}

	public override void Use(AbilitySystem owner, Vector3 targetVecPos)
	{
		
	}
}
