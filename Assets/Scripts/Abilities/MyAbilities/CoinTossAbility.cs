using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CoinTossAbility", menuName = "Abilities/Coin Toss")]
public class CoinTossAbility : CharacterAbility
{
	[SerializeField] private int coinCount = 5;
	[SerializeField] private GameObject coinProjectile;
	[SerializeField] private float launchForce = 20f;
	public override void Init()
	{
		coinCount = 5;
	}


	public override void Use(AbilitySystem owner)
	{
		if (coinCount > 0)
		{

			GameObject projectile = Instantiate(coinProjectile, owner.GetCastposition() + owner.GetAimDirection() * 1.05f, Quaternion.identity);

			Rigidbody rb = projectile.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.velocity = owner.GetAimDirection() * launchForce;
			}

			CoinProjectile projectileScript = projectile.GetComponent<CoinProjectile>();
			if (projectileScript != null)
			{
				projectileScript.SetAnchorCallback(CreateDistraction);
			}
			coinCount--;
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
		
	}

	private void CreateDistraction(Vector3 position, Vector3 normal)
	{
		//Callback to then create a distraction or whatever is needed
	}
}
