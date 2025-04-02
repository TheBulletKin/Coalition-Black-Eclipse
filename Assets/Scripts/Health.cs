using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable
{
	[SerializeField] private int maxHealth = 100;
	[SerializeField] private int currentHealth;

	public event Action<Health> OnEntityDeath;
	

	private void Start()
	{
		currentHealth = maxHealth;
	}
	public void Die()
	{
		Debug.Log(gameObject.name + " died");
		OnEntityDeath?.Invoke(this);
		Destroy(gameObject);
	}

	public void TakeDamage(int damage)
	{

		currentHealth -= Math.Abs(damage);
		Debug.Log(gameObject.name + " took " + damage + " damage");
		if (currentHealth < 0)
		{
			currentHealth = 0;
			Die();
		} 
	}

	
}
