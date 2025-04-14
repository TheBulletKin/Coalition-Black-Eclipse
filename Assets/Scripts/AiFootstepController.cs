using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Temporary component used to ready enemy entity movement
/// </summary>
public class AiFootstepController : MonoBehaviour
{
	[Header("Footstep settings")]
	public float stepDistance = 2f;
	private float distanceMoved = 0f;
	private Vector3 lastPosition;

	private void Update()
	{
		float moved = Vector3.Distance(transform.position, lastPosition);
		distanceMoved += moved;

		if (distanceMoved >= stepDistance)
		{
			AudioManager.instance.PlaySound(SoundType.FOOTSTEP, MixerBus.FOOTSTEP_ENEMY, new Vector3(transform.position.x, transform.position.y - 0.7f, transform.position.z), transform);
			distanceMoved = 0f;
		}

		lastPosition = transform.position;
	}
}
