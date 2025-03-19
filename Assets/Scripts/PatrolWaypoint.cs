using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolWaypoint : MonoBehaviour
{
    public Transform waypointTransform;
    public Vector3 position;
    public Vector3 rotation;
    public float waitDuration = 0.0f;

	private void Start()
	{
		waypointTransform = transform;
		position = transform.position;
	}
}
