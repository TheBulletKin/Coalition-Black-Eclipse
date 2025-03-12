using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySightSensor : MonoBehaviour
{

	public Health currentTarget { get; private set; }
	public List<Health> visibleEntities;
	private float pingInteval = 0.25f;
	private float pingTimer = 0.0f;
	[Tooltip("How detected the player is, from 0 to 1")]
	[field: SerializeField]
	public float detectionValue { get; private set; }
	[SerializeField] private float detectionIncreaseRate = 5f;
	[SerializeField] private float detectionDecreaseRate = 2f;
	[SerializeField] private float detectionThreshold = 100f;

	[SerializeField] private float detectionAngle = 60f;
	[SerializeField] private float maxDetectionDistance = 40f;
	[SerializeField] private float closeDetectionFalloffDistance = 10f;
	[SerializeField] private float detectionRateModifier;
	public bool entityIsDetected { get; private set; }
	public bool isEngagingEnemy = false;


	[SerializeField] private LayerMask ignoreMask;

	[SerializeField] private float maxWeaponRange = 30f;
	[SerializeField] private int weaponDamage = 30;
	[SerializeField] private float fireTimer = 0f;
	[SerializeField] private float fireCooldown = 0.5f;

	private Ray ray;

	private void Awake()
	{
		detectionValue = 0.0f;
		detectionRateModifier = 1.0f;



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


			if (enemyIsInVisibleRange)
			{
				float distanceToVisible = (visibleEntities[0].transform.position - transform.position).magnitude;

				//Within 10 metres, maximum gain
				//Beyond 30 meters, no gain
				if (distanceToVisible >= maxDetectionDistance)
				{
					//Decrement detection
					if (entityIsDetected == false)
					{
						detectionValue -= detectionIncreaseRate * deltaTime;
						detectionValue = Mathf.Clamp(detectionValue, 0, detectionThreshold);
					}

				}
				else if (distanceToVisible < maxDetectionDistance && distanceToVisible >= 0f) //Within detection range
				{
					detectionRateModifier = Mathf.Lerp(1f, 0f,
					(distanceToVisible - closeDetectionFalloffDistance) / (maxDetectionDistance - closeDetectionFalloffDistance));
					//Want distance / max distance but normalised to ignore the 10 units close falloff.
					detectionValue += detectionIncreaseRate * deltaTime * detectionRateModifier;
					detectionValue = Mathf.Clamp(detectionValue, 0, detectionThreshold);
				}


				if (detectionValue >= detectionThreshold)
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
					detectionValue -= detectionIncreaseRate * deltaTime;
					detectionValue = Mathf.Clamp(detectionValue, 0, detectionThreshold);
				}

			}

			pingTimer = 0.0f;
		}

		RaycastHit hit;
		if (currentTarget != null && isEngagingEnemy && TargetInLineOfSight(out hit) && TargetInWeaponRange())
		{
			fireTimer += deltaTime;
			if(fireTimer >= fireCooldown)
			{
				Health targetHealth = currentTarget.GetComponent<Health>();
				currentTarget.TakeDamage(weaponDamage);
				fireTimer = 0.0f;
			}
		}

	

		if (currentTarget == null)
		{
			visibleEntities.Remove(currentTarget);
			entityIsDetected = false;
			isEngagingEnemy = false;
		}
	}

	//One test to see if any teammate is in view
	public bool Ping(ControllableEntity target)
	{

		Health entityHealthComponent = target.health;
		//Ray from current ai to target
		ray = new Ray(this.transform.position, target.characterModel.transform.position - this.transform.position);

		var dir = new Vector3(ray.direction.x, 0, ray.direction.z);

		var angle = Vector3.Angle(dir, this.transform.forward);

		//Enemy outside of vision cone
		if (angle > detectionAngle)
		{
			if (visibleEntities.Contains(entityHealthComponent))
			{
				entityHealthComponent.OnEntityDeath -= OnEnemyDeath;
				visibleEntities.Remove(entityHealthComponent);
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
			if (visibleEntities.Contains(entityHealthComponent))
			{
				entityHealthComponent.OnEntityDeath -= OnEnemyDeath;
				visibleEntities.Remove(entityHealthComponent);
			}
			return false;
		}

		//When visible
		if (hit.collider.tag == "Teammate")
		{
			EntityVisibility vis = hit.collider.gameObject.GetComponentInParent<EntityVisibility>();
			if (vis.GetVisibilityMod() > 0) //Can be seen
			{
				if (!visibleEntities.Contains(entityHealthComponent))
				{
					visibleEntities.Add(entityHealthComponent);
					entityHealthComponent.OnEntityDeath += OnEnemyDeath;
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

	public bool TargetInLineOfSight(out RaycastHit outHit)
	{
		//As it casts a ray from the current location to the targetted teammate,
		//I just need to get the first object hit and compare tags
		//Ignore the enemy itself and hit everything else.
		if (Physics.Raycast(ray, out RaycastHit Hit, 70, ignoreMask))
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
		if ((currentTarget.transform.position - transform.position).magnitude <= maxWeaponRange - 5f) //5 as some room before needing to move again
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
			closestDistance = maxWeaponRange;
		}




		return closestEnemy;

	}



	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * 100);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * 100);
	}
}
