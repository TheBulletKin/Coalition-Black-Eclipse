using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AiSightSensor : MonoBehaviour
{

	public Health currentTarget { get; private set; }
	public ShootingSystem shootingSystem;
	public List<Health> visibleEntities;
	private float pingInteval = 0.25f;
	private float pingTimer = 0.0f;
	[Tooltip("How detected the player is, from 0 to 1")]
	[field: SerializeField]
	public float detectionValue { get; private set; }


	[SerializeField] private AiVisionConfig visionConfig;
	public bool entityIsDetected { get; private set; }
	public bool isEngagingEnemy = false;


	private Ray ray;

	private void Awake()
	{
		detectionValue = 0.0f;
		
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		pingTimer += deltaTime;
		if (pingTimer >= pingInteval)
		{
			//If any enemy is in sight, increment detection. Reduce if not visible

			bool enemyIsInVisibleRange = false;
			foreach (ControllableEntity target in EntityManager.Instance.playerTeammates)
			{
				if (Ping(target)) //If at least one enemy is visible, will continue and change the detection value
				{
					enemyIsInVisibleRange = true;
					break;
				}
			}

			foreach (Decoy decoy in EntityManager.Instance.playerDecoys)
			{
				if (Ping(decoy)) //If at least one decoy is visible, will continue and change the detection value
				{
					enemyIsInVisibleRange = true;
					break;
				}
			}


			if (enemyIsInVisibleRange)
			{
				float distanceToVisible = (visibleEntities[0].transform.position - transform.position).magnitude;

				//Within 10 metres, maximum gain
				//Beyond 30 meters, no gain
				if (distanceToVisible >= visionConfig.maxDetectionDistance)
				{
					//Decrement detection
					if (entityIsDetected == false)
					{
						detectionValue -= visionConfig.detectionIncreaseRate * deltaTime;
						detectionValue = Mathf.Clamp(detectionValue, 0, visionConfig.detectionThreshold);
					}

				}
				else if (distanceToVisible < visionConfig.maxDetectionDistance && distanceToVisible >= 0f) //Within detection range
				{
					visionConfig.detectionRateModifier = Mathf.Lerp(1f, 0f,
					(distanceToVisible - visionConfig.closeDetectionFalloffDistance) / (visionConfig.maxDetectionDistance - visionConfig.closeDetectionFalloffDistance));
					//Want distance / max distance but normalised to ignore the 10 units close falloff.
					detectionValue += visionConfig.detectionIncreaseRate * deltaTime * visionConfig.detectionRateModifier;
					detectionValue = Mathf.Clamp(detectionValue, 0, visionConfig.detectionThreshold);
				}


				if (detectionValue >= visionConfig.detectionThreshold)
				{
					entityIsDetected = true;
					currentTarget = visibleEntities[0];
				}
			}
			else
			{
				//Ensures that after detection the ai won't just forget the player's position
				if (entityIsDetected == false)
				{
					detectionValue -= visionConfig.detectionIncreaseRate * deltaTime;
					detectionValue = Mathf.Clamp(detectionValue, 0, visionConfig.detectionThreshold);
				}

			}

			pingTimer = 0.0f;
		}


		RaycastHit hit;
		if (currentTarget != null && isEngagingEnemy && TargetInLineOfSight(out hit) && TargetInWeaponRange())
		{

			shootingSystem.Fire(currentTarget.transform);
			/*
			fireTimer += deltaTime;
			if(fireTimer >= fireCooldown)
			{
				Health targetHealth = currentTarget.GetComponent<Health>();
				currentTarget.TakeDamage(weaponDamage);
				fireTimer = 0.0f;
			}*/
		}

	

		if (currentTarget == null)
		{
			visibleEntities.Remove(currentTarget);
			entityIsDetected = false;
			isEngagingEnemy = false;
		}
	}

	public bool Ping(Decoy target)
	{
		return TestVisibility(target.health);
	}

	public bool Ping(ControllableEntity target)
	{

		return TestVisibility(target.health);
	}

	public bool TestVisibility(Health target)
	{		
		//Ray from current ai to target
		ray = new Ray(this.transform.position, target.transform.position - this.transform.position);

		var dir = new Vector3(ray.direction.x, 0, ray.direction.z);

		var angle = Vector3.Angle(dir, this.transform.forward);

		//Enemy outside of vision cone
		if (angle > visionConfig.detectionAngle)
		{
			if (visibleEntities.Contains(target))
			{
				target.OnEntityDeath -= OnEnemyDeath;
				visibleEntities.Remove(target);
			}
			return false;
		}

		RaycastHit hit;

		//LOS check - Out lets it alter the variable above. Similar to ref but doesn't need to be initialized.
		if (TargetInLineOfSight(out hit))
		{

		}
		else
		{
			if (visibleEntities.Contains(target))
			{
				target.OnEntityDeath -= OnEnemyDeath;
				visibleEntities.Remove(target);
			}
			return false;
		}

		//When visible
		if (hit.collider.tag == "Teammate")
		{
			EntityVisibility vis = hit.collider.gameObject.GetComponentInParent<EntityVisibility>();
			if (vis.GetVisibilityMod() > 0) //Can be seen
			{
				if (!visibleEntities.Contains(target))
				{
					visibleEntities.Add(target);
					target.OnEntityDeath += OnEnemyDeath;
				}
				return true;
			}
			else
			{
				return false;
			}

		}

		return false;
	}
	
	//One test to see if any teammate is in view
	

	public bool TargetInLineOfSight(out RaycastHit outHit)
	{
		//As it casts a ray from the current location to the targetted teammate,
		//I just need to get the first object hit and compare tags
		//Ignore the enemy itself and hit everything else.
		if (Physics.Raycast(ray, out RaycastHit Hit, 70, visionConfig.ignoreMask))
		{
			if (Hit.collider.tag == "Teammate")
			{
				outHit = Hit;
				return true;
			}
			else
			{
				outHit = new RaycastHit();
				return false;
			}
		}
		else
		{
			outHit = new RaycastHit();
			return false;
		}
	}

	public float GetDetectionValue()
	{
		return detectionValue;
	}

	private void OnEnemyDeath(Health deadEntity)
	{
		visibleEntities.Remove(deadEntity);
		isEngagingEnemy = false;
		Debug.Log("Removed entity from list");
	}

	public bool TargetInWeaponRange()
	{
		if ((currentTarget.transform.position - transform.position).magnitude <= shootingSystem.weaponConfig.weaponRange - 5f) //5 as some room before needing to move again
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private Health GetClosestEnemySeen()
	{
		Vector3 targetVector;
		float closestDistance;
		Health closestEnemy;
		if (visibleEntities.Count >= 1)
		{
			closestEnemy = visibleEntities[0];
			if (closestEnemy == null)
			{
				visibleEntities.Remove(closestEnemy);
			}
			else
			{
				targetVector = closestEnemy.transform.position - transform.position;
				closestDistance = targetVector.magnitude;

				for (int i = 1; i < visibleEntities.Count; i++)
				{
					if (visibleEntities[i] == null)
					{
						visibleEntities.RemoveAt(i);
					}
					else
					{
						targetVector = visibleEntities[i].transform.position - transform.position;
						if (targetVector.magnitude < closestDistance)
						{
							closestEnemy = visibleEntities[i];
							closestDistance = targetVector.magnitude;
						}
					}
				}

			}
		}
		else
		{
			closestEnemy = null;
			closestDistance = shootingSystem.weaponConfig.weaponRange;
		}




		return closestEnemy;

	}



	private void OnDrawGizmosSelected()
	{		
		
		
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, visionConfig.maxDetectionDistance);
		Gizmos.DrawWireSphere(transform.position, visionConfig.closeDetectionFalloffDistance);

		Gizmos.color = Color.red;
		Vector3 forward = transform.forward * visionConfig.maxDetectionDistance;
		Quaternion leftRayRotation = Quaternion.AngleAxis(-visionConfig.detectionAngle / 2, Vector3.up);
		Quaternion rightRayRotation = Quaternion.AngleAxis(visionConfig.detectionAngle / 2, Vector3.up);
		Gizmos.DrawRay(transform.position, leftRayRotation * forward);
		Gizmos.DrawRay(transform.position, rightRayRotation * forward);
	}
	
}
