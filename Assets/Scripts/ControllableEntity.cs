using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControllableEntity : MonoBehaviour
{
	public bool isControlledByPlayer = false;
	public int teammateID;
	
	public GameObject characterModel;
	public Transform cameraPos;

	IToggleable[] toggleableComponents;
	[SerializeField] public NavMeshAgent agent { get; private set; }
	[SerializeField] public AIMovement aiMovement { get; private set; }
	[SerializeField] public AiCommandListener commandListener { get; private set; }
	[SerializeField] public EntityVisibility entityVisibility { get; private set; }
	[SerializeField] public ShootingSystem shootingSystem { get; private set; }
	[SerializeField] public AbilitySystem abilitySystem { get; private set; }
	[SerializeField] public Health health { get; private set; }
	[SerializeField] public AiDetectAndEngage aiDetection { get; private set; }

	private void Awake()
	{
		toggleableComponents = GetComponents<IToggleable>();
		agent = GetComponent<NavMeshAgent>();
		aiMovement = GetComponent<AIMovement>();
		commandListener = GetComponent<AiCommandListener>();
		entityVisibility = GetComponent<EntityVisibility>();
		shootingSystem = GetComponent<ShootingSystem>();
		abilitySystem = GetComponent<AbilitySystem>();
		health = GetComponent<Health>();
		aiDetection = GetComponent<AiDetectAndEngage>();


	}

	public void TakeControl()
	{
		if (toggleableComponents == null)
		{
			toggleableComponents = GetComponents<IToggleable>();
		}
		foreach (IToggleable item in toggleableComponents)
		{
			item.EnableControl();
		}
		agent.enabled = false;
	}

	public void LoseControl()
	{
		if (toggleableComponents == null)
		{
			toggleableComponents = GetComponents<IToggleable>();
		}
		foreach (IToggleable item in toggleableComponents)
		{
			item.DisableControl();
		}
		agent.enabled = true;
	}

	public void HandleDeath()
	{
		//Previously didn't unsubscribe to the fire and shoot methods when dead. This is a temp fix.
		shootingSystem.DisableControl();
	}

}
