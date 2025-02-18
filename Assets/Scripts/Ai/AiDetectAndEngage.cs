using System;
using System.Collections.Generic;
using UnityEngine;

public class AiDetectAndEngage : MonoBehaviour, IToggleable
{

	[SerializeField] private float detectionRange = 10f;
	[SerializeField] private float detectionAngle = 45f;
	[SerializeField] private float engagementMaxRange;
	[SerializeField] private float engagementOptimalRange;

	[SerializeField] private float fireTimer = 0f;
	[SerializeField] private float fireCooldown = 1f;

	[SerializeField] private float detectionScanTimer = 0f;
	[SerializeField] private float scanCooldown = 0.08f;

	[SerializeField] private int weaponDamage = 10;

	[SerializeField] private LayerMask enemyLayer;
	[SerializeField] private LayerMask obstructionLayers;

	[SerializeField] private List<Health> enemiesSeen;	

	private void Start()
	{
		
	}

	private void Update()
	{
		if (detectionScanTimer <= 0f)
		{
			//Should start a detection scan
			DistanceCheck();
			VisionConeCheck();
			ObstructionCheck();
			detectionScanTimer = scanCooldown;
		}
		else
		{
			//Waiting for scan
			detectionScanTimer -= Time.deltaTime;
		}

		if (enemiesSeen.Count >= 1)
		{
			if (fireTimer <= 0F)
			{
				//Able to fire
				Health entityToTarget = GetClosestEnemySeen();
				if (entityToTarget != null)
				{
					EngageTarget(entityToTarget.transform);
				}


				fireTimer = fireCooldown;
			}
			else
			{
				//Waiting to fire
				fireTimer -= Time.deltaTime;
			}
		}
		else
		{
			fireTimer = fireCooldown;
		}
		


	}


	private Health GetClosestEnemySeen()
	{
		Vector3 targetVector;
		float closestDistance;
		Health closestEnemy;
		if (enemiesSeen.Count >= 1)
		{
			closestEnemy = enemiesSeen[0];
			targetVector = closestEnemy.transform.position - transform.position;
			closestDistance = targetVector.magnitude;
		}
		else
		{
			closestEnemy = null;
			closestDistance = detectionRange + 1f;
		}


		for (int i = 1; i < enemiesSeen.Count; i++)
		{
			targetVector = enemiesSeen[i].transform.position - transform.position;
			if (targetVector.magnitude < closestDistance)
			{
				closestEnemy = enemiesSeen[i];
				closestDistance = targetVector.magnitude;
			}
		}

		return closestEnemy;

	}

	/// <summary>
	/// Changes the enemiesSeen to only contain those in range
	/// </summary>
	private void DistanceCheck()
	{

		//First clear enemies that aren't in range
		for (int i = enemiesSeen.Count - 1; i >= 0; i--)
		{
			Vector3 targetVector = enemiesSeen[i].transform.position - transform.position;
			if (targetVector.magnitude > (detectionRange))
			{
				enemiesSeen[i].OnEnemyDeath -= OnEnemyDeath;
				enemiesSeen.RemoveAt(i);
			}
		}

		//Get all colliders actually in range
		Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);

		foreach (Collider collider in enemiesInRange)
		{
			Health entityHealthComponent = collider.GetComponent<Health>();

			if (entityHealthComponent != null)
			{
				//Target in range but not in enemiesSeen list
				if (!enemiesSeen.Contains(entityHealthComponent))
				{
					enemiesSeen.Add(entityHealthComponent);
					entityHealthComponent.OnEnemyDeath += OnEnemyDeath;
				}

			}

		}

	}

	/// <summary>
	/// Removes enemiesSeen that are not within the vision cone
	/// </summary>
	private void VisionConeCheck()
	{
		for (int i = enemiesSeen.Count - 1; i >= 0; i--)
		{

			Vector3 directionToTarget = enemiesSeen[i].transform.position - transform.position;
			Vector3 forwardDirection = transform.forward;
			float angle = Vector3.Angle(forwardDirection, directionToTarget.normalized);

			if (angle >= detectionAngle)
			{
				enemiesSeen[i].OnEnemyDeath -= OnEnemyDeath;
				enemiesSeen.RemoveAt(i);
			}
		}
	}

	/// <summary>
	/// Removes enemiesSeen that are obstructed by objects
	/// </summary>
	private void ObstructionCheck()
	{
		for (int i = enemiesSeen.Count - 1; i >= 0; i--)
		{
			Vector3 vectorToTarget = enemiesSeen[i].transform.position - transform.position;
			Ray ray = new Ray(transform.position, vectorToTarget.normalized);
			if (Physics.Raycast(ray, out RaycastHit hit, vectorToTarget.magnitude, obstructionLayers))
			{
				enemiesSeen[i].OnEnemyDeath -= OnEnemyDeath;
				enemiesSeen.RemoveAt(i);
			}
		}
	}

	/// <summary>
	/// Fire on the target, raycasting then dealing damage
	/// </summary>
	/// <param name="targetTransform"></param>
	private void EngageTarget(Transform targetTransform)
	{
		Vector3 vectorToTarget = targetTransform.position - transform.position;
		Ray fireRay = new Ray(transform.position, vectorToTarget.normalized);

		if (Physics.Raycast(fireRay, out RaycastHit hit, vectorToTarget.magnitude, enemyLayer))
		{
			IDamagable damagableTarget = hit.collider.GetComponent<IDamagable>();

			if (damagableTarget != null)
			{
				damagableTarget.TakeDamage(weaponDamage);
			}
		}
	}

	void OnDrawGizmosSelected()
	{

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, detectionRange);

		Gizmos.color = Color.red;
		Vector3 forward = transform.forward * detectionRange;
		Quaternion leftRayRotation = Quaternion.AngleAxis(-detectionAngle / 2, Vector3.up);
		Quaternion rightRayRotation = Quaternion.AngleAxis(detectionAngle / 2, Vector3.up);
		Gizmos.DrawRay(transform.position, leftRayRotation * forward);
		Gizmos.DrawRay(transform.position, rightRayRotation * forward);	


		
	}

	private void OnEnemyDeath(Health deadEntity)
	{
		enemiesSeen.Remove(deadEntity);
		Debug.Log("Removed entity from list");
	}

	public void DisableControl()
	{
		enabled = true;
	}

	public void EnableControl()
	{
		enabled = false;
	}
}
