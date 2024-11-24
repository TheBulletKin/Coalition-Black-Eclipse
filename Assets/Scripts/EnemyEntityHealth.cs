using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntityHealth : MonoBehaviour, IDamagable
{
	[SerializeField] private int maxHealth = 100;
	[SerializeField] private int currentHealth;

	public event Action<EnemyEntityHealth> OnEnemyDeath;

	private void Start()
	{
		currentHealth = maxHealth;
	}
	public void Die()
	{
		Debug.Log("Entity died");
		OnEnemyDeath?.Invoke(this);
		Destroy(gameObject);
	}

	public void TakeDamage(int damage)
	{

		currentHealth -= Math.Abs(damage);
		Debug.Log("Entity took " + damage + " damage");
		if (currentHealth < 0)
		{
			currentHealth = 0;
			Die();
		} 
	}

	
}
