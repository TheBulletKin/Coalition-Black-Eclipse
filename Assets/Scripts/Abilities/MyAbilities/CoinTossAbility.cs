using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "CoinTossAbility", menuName = "Abilities/Coin Toss")]
public class CoinTossAbility : CharacterAbility
{	
	[SerializeField] private GameObject coinProjectile;
	[SerializeField] private float launchForce = 20f;
	[SerializeField] private Sound emittedSound;



	public override void Init(AbilitySystem owner)
	{
		base.Init(owner);
	}


	public override void Use(AbilitySystem owner)
	{
		if (currentAbilityCount > 0)
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
		SoundEmitterHandler.instance.EmitDetectableSound(emittedSound, targetVecPos);
	}

	private void CreateDistraction(Vector3 position, Vector3 normal)
	{
		SoundEmitterHandler.instance.EmitDetectableSound(emittedSound, position);
	}

	
}
