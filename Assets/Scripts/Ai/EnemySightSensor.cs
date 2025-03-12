using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySightSensor : MonoBehaviour
{
	public Transform currentTarget { get; private set; }
	public List<Transform> playerEntities;
	public List<Transform> visibleEntities;
	private float pingInteval = 0.25f;
	private float pingTimer = 0.0f;
	[Tooltip("How detected the player is, from 0 to 1")]
	[field: SerializeField]
	public float detectionValue { get; private set; }
	[SerializeField] private float detectionIncreaseRate = 5f;
	[SerializeField] private float detectionDecreaseRate = 2f;
	[SerializeField] private float detectionThreshold = 100f;
	public bool entityIsDetected { get; private set; }

	[SerializeField] private LayerMask _ignoreMask;

	private Ray ray;

	private void Awake()
	{
		detectionValue = 0.0f;
	}

	private void Update()
	{
		pingTimer += Time.deltaTime;
		if (pingTimer >= pingInteval)
		{
			//If any enemy is in sight, increment detection. Reduce if not visible
			if (Ping())
			{
				detectionValue += detectionIncreaseRate * Time.deltaTime;
				detectionValue = Mathf.Clamp(detectionValue, 0, detectionThreshold);

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
					detectionValue -= detectionIncreaseRate * Time.deltaTime;
					detectionValue = Mathf.Clamp(detectionValue, 0, detectionThreshold);
				}

			}

			pingTimer = 0.0f;
		}
	}

	//One test to see if any teammate is in view
	public bool Ping()
	{
		foreach (Transform target in playerEntities)
		{
			//Ray from current ai to target
			ray = new Ray(this.transform.position, target.position - this.transform.position);

			var dir = new Vector3(ray.direction.x, 0, ray.direction.z);

			var angle = Vector3.Angle(dir, this.transform.forward);

			//Vision cone check
			if (angle > 60)
			{
				if (visibleEntities.Contains(target))
				{
					visibleEntities.Remove(target);
				}
				return false;
			}

			//LOS check
			if (!Physics.Raycast(ray, out var hit, 70, ~_ignoreMask))
			{
				if (visibleEntities.Contains(target))
				{
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
					}
					return true;
				}
				else
				{
					return false;
				}

			}
		}



		return false;
	}

	public float GetDetectionValue()
	{
		return detectionValue;
	}



	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * 100);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * 100);
	}
}
