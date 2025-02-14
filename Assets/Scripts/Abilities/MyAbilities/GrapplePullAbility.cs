using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GrapplePullAbility", menuName = "Abilities/Grapple Pull")]
public class GrapplePullAbility : CharacterAbility
{
	public override void Init()
	{
		
	}

	public override void Use(AbilitySystem owner, GameObject target)
	{
		Debug.Log("Used grapple ability");
	}
}
