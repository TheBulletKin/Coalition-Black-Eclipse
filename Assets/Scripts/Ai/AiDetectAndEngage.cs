using System;
using System.Collections.Generic;
using UnityEngine;

public class AiDetectAndEngage : MonoBehaviour, IToggleable
{
	public ShootingSystem shootingSystem;
	[SerializeField] private float detectionRange = 10f;
	[SerializeField] private float detectionAngle = 45f;
	[SerializeField] private float engagementMaxRange;
	[SerializeField] private float engagementOptimalRange;

	[SerializeField] private float fireTimer = 0f;
	[SerializeField] private float fireCooldown = 1f;

	[SerializeField] private float detectionScanTimer = 0f;
	[SerializeField] private float scanCooldown = 0.08f;

	[Tooltip("How detected the player is, from 0 to 1")]
	[SerializeField] private float detectionValue = 0f;
	[SerializeField] private float detectionIncreaseRate = 5f;
	[SerializeField] private float detectionDecreaseRate = 2f;
	[SerializeField] private float detectionThreshold = 100f;

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
			//DecrementDetection();
			detectionScanTimer = scanCooldown;
		}
		else
		{
			//Waiting for scan
			detectionScanTimer -= Time.deltaTime;
		}

		if (enemiesSeen.Count >= 1)
		{
			
			Health entityToTarget = GetClosestEnemySeen();
			if (entityToTarget != null)
			{
				EngageTarget(entityToTarget.transform);
				
			}
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
			if (closestEnemy == null)
			{
				enemiesSeen.Remove(closestEnemy);
			}
			else
			{
				targetVector = closestEnemy.transform.position - transform.position;
				closestDistance = targetVector.magnitude;

				for (int i = 1; i < enemiesSeen.Count; i++)
				{
					if (enemiesSeen[i] == null)
					{
						enemiesSeen.RemoveAt(i);
					}
					else
					{
						targetVector = enemiesSeen[i].transform.position - transform.position;
						if (targetVector.magnitude < closestDistance)
						{
							closestEnemy = enemiesSeen[i];
							closestDistance = targetVector.magnitude;
						}
					}
				}

			}
		}
		else
		{
			closestEnemy = null;
			closestDistance = shootingSystem.weaponConfig.weaponRange + 1f;
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
			if (enemiesSeen[i] == null)
			{
				enemiesSeen.RemoveAt(i);
			}
			else
			{
				Vector3 targetVector = enemiesSeen[i].transform.position - transform.position;
				if (targetVector.magnitude > (shootingSystem.weaponConfig.weaponRange))
				{
					enemiesSeen[i].OnEntityDeath -= OnEnemyDeath;
					enemiesSeen.RemoveAt(i);
				}
			}

		}

		//Get all colliders actually in range
		Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, shootingSystem.weaponConfig.weaponRange, enemyLayer);

		foreach (Collider collider in enemiesInRange)
		{
			Health entityHealthComponent = collider.GetComponentInParent<Health>();

			if (entityHealthComponent != null)
			{
				//Target in range but not in enemiesSeen list
				if (!enemiesSeen.Contains(entityHealthComponent))
				{
					enemiesSeen.Add(entityHealthComponent);
					entityHealthComponent.OnEntityDeath += OnEnemyDeath;
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
				if (enemiesSeen[i] == null)
				{
					enemiesSeen.RemoveAt(i);
				}
				else
				{
					enemiesSeen[i].OnEntityDeath -= OnEnemyDeath;
					enemiesSeen.RemoveAt(i);
				}
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
			if (enemiesSeen[i] == null)
			{
				enemiesSeen.RemoveAt(i);
			}
			else
			{
				//If unable to see enemy, remove from enemiesSeen
				Vector3 vectorToTarget = enemiesSeen[i].transform.position - transform.position;
				Ray ray = new Ray(transform.position, vectorToTarget.normalized);
				if (Physics.Raycast(ray, out RaycastHit hit, vectorToTarget.magnitude, obstructionLayers))
				{
					enemiesSeen[i].OnEntityDeath -= OnEnemyDeath;
					enemiesSeen.RemoveAt(i);
				}
				else
				{
					//detectionValue += detectionIncreaseRate * Time.deltaTime;
					//detectionValue = Mathf.Clamp(detectionValue, 0, detectionThreshold);
				}
			}
		}
	}

	private void DecrementDetection()
	{
		//If nobody is seen, start lowering that detection value
		if (enemiesSeen.Count == 0)
		{
			//detectionValue -= detectionDecreaseRate * Time.deltaTime;
			//detectionValue = Mathf.Clamp(detectionValue, 0, detectionThreshold);
		}
	}

	/// <summary>
	/// Fire on the target, raycasting then dealing damage
	/// </summary>
	/// <param name="targetTransform"></param>
	private void EngageTarget(Transform targetTransform)
	{

		shootingSystem.Fire(targetTransform);

	}

	void OnDrawGizmosSelected()
	{

		//Gizmos.color = Color.yellow;
		//Gizmos.DrawWireSphere(transform.position, detectionRange);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, shootingSystem.weaponConfig.weaponRange);

		Gizmos.color = Color.red;
		Vector3 forward = transform.forward * shootingSystem.weaponConfig.weaponRange;
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
