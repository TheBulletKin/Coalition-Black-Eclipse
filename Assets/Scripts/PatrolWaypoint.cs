using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolWaypoint : MonoBehaviour
{
	public Transform orientation;
    public float waitDuration = 0.0f;
	public int patrolPointIndex = 0;

	private void Start()
	{		
		
	}

	private void OnDrawGizmos()
	{
		// Set Gizmo color
		Gizmos.color = Color.green;

		// Draw a line from the AI's position in its forward direction
		Vector3 forward = orientation.transform.forward;
		float lineLength = 3f; // Length of the line

		// Draw the line
		Gizmos.DrawLine(orientation.transform.position, orientation.transform.position + forward * lineLength);

		// Draw a small arrowhead to indicate direction
		Vector3 arrowTip = orientation.transform.position + forward * lineLength;
		Vector3 right = Quaternion.Euler(0, 20, 0) * -forward;
		Vector3 left = Quaternion.Euler(0, -20, 0) * -forward;

		Gizmos.DrawLine(arrowTip, arrowTip + right * 0.5f);
		Gizmos.DrawLine(arrowTip, arrowTip + left * 0.5f);

		Gizmos.DrawSphere(transform.position, 0.2f);
	}
}
