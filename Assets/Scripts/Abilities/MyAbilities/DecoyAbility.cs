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


	public override void Use(AbilitySystem owner, GameObject target = null, RaycastHit targetPos = default)
	{
		Instantiate(decoyPrefab, targetPos.point, Quaternion.identity);
	}
}
