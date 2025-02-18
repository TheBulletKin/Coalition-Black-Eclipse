using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySightSensor : MonoBehaviour
{
	public Transform Player { get; private set; }

	[SerializeField] private LayerMask _ignoreMask;

	private Ray ray;

	private void Awake()
	{
		Player = GameObject.Find("Player").transform;
	}

	//One test to see if the player is in view
	public bool Ping()
	{
		if (Player == null)
			return false;

		ray = new Ray(this.transform.position, Player.position - this.transform.position);

		var dir = new Vector3(ray.direction.x, 0, ray.direction.z);

		var angle = Vector3.Angle(dir, this.transform.forward);

		if (angle > 60)
			return false;

		if (!Physics.Raycast(ray, out var hit, 100, ~_ignoreMask))
		{
			return false;
		}

		if (hit.collider.tag == "Player")
		{
			EntityVisibility vis = hit.collider.gameObject.GetComponent<EntityVisibility>();
			if (vis.GetVisibilityMod() > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
			
		}

		return false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * 100);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * 100);
	}
}
