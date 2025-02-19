using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleProjectile : MonoBehaviour, IDamagable
{
	public bool isThrown = false;

	public void Die()
	{
		
	}

	//Clever use of damageable interface to allow for destruction upon shooting
	public void TakeDamage(int damage)
	{
		if (isThrown)
		{
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		
		if (isThrown)
		{
			Destroy(gameObject);
		}
	}
}
