using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour, IToggleable
{
	private NavMeshAgent agent;

	[Serializable]
	public class RotationAndLookConfig
	{

		public float lookRotationSpeed = 0.01f;
	}

	[Header("Look rotation attributes")]
	[SerializeField] private RotationAndLookConfig rotationAndLookConfig;
	
	private bool isPlayerControlled = false;

	[Header("Footstep settings")]
	public float stepDistance = 2f;
	private float distanceMoved = 0f;
	private Vector3 lastPosition;
	[Tooltip("Whether it emits a sound that ai entities can detect")]
	[SerializeField] private bool emitsDetectableSound;
	public Sound detectableFootstepSound;
	[Tooltip("Whether it emits a sound that the player can hear")]
	[SerializeField] private bool emitsAudibleSound;

	[Header("Debug")]
	[SerializeField] private Vector3 lookTarget;
	[SerializeField] private bool isPieingTarget = false;
	[SerializeField] private float lookRotationDuration = 1f;


	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		lastPosition = transform.position;
	}

	public void MoveTo(Vector3 targetPosition)
	{
		if (agent != null)
		{
			agent.SetDestination(targetPosition);
		}
	}

	public void StopMovement()
	{
		if (agent != null)
		{
			agent.ResetPath();
		}
	}

	public void SetLooking(bool state)
	{
		isPieingTarget = state;
		if (state == true)
		{
			agent.updateRotation = false;
		}
		else
		{
			agent.updateRotation = true;
		}
	}

	public void SetLooking(Vector3 targetPosition)
	{
		lookTarget = targetPosition;
		isPieingTarget = true;
		agent.updateRotation = false;
	}

	private void Update()
	{
		if (!isPlayerControlled)
		{
			HandleLook();

			float moved = Vector3.Distance(transform.position, lastPosition);
			distanceMoved += moved;

			if (distanceMoved >= stepDistance)
			{
				CreateSound(new Vector3(transform.position.x, transform.position.y - 0.7f, transform.position.z));

				distanceMoved = 0f;
			}

			lastPosition = transform.position;
		}


	}

	private void HandleLook()
	{
		if (isPieingTarget)
		{
			Vector3 lookDirection = (lookTarget - gameObject.transform.position).normalized;
			Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationAndLookConfig.lookRotationSpeed * Time.deltaTime);

			transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
		}
	}

	private void CreateSound(Vector3 position)
	{
		if (emitsAudibleSound)
		{
			SoundEmitterHandler.instance.EmitAudibleSound(SoundType.FOOTSTEP, MixerBus.FOOTSTEP_PLAYER, position, null);
		}


		if (emitsDetectableSound)
		{
			SoundEmitterHandler.instance.EmitDetectableSound(detectableFootstepSound, position);
		}
	}

	public void DisableControl()
	{
		isPieingTarget = false;
		isPlayerControlled = false;
		if (agent)
		{
			agent.updateRotation = true;
		}
	}

	public void EnableControl()
	{
		if (isPieingTarget)
		{
			agent.updateRotation = false;
		}
		isPlayerControlled = true;
	}
}
