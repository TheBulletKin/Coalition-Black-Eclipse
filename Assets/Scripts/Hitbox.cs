using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour, IDamagable
{
	public Health parentHealth;
	public HitboxTypes hitboxType;

	public void Die()
	{
		
	}

	public void TakeDamage(int damage)
	{
		switch (hitboxType)
		{
			case HitboxTypes.BODY:
				damage *= 1;
				break;
			case HitboxTypes.HEAD:
				damage *= 2;
				break;
			default:
				break;
		}

		parentHealth.TakeDamage(damage);
	}
}
