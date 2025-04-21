using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControllableEntity : MonoBehaviour, IInitialisable
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


	private List<IStatusEffect> activeStatusEffects = new List<IStatusEffect>();

	
	/// <summary>
	/// Requires: InputManager
	/// </summary>
	/// <returns></returns>
	public void Initialize()
	{
		toggleableComponents = GetComponents<IToggleable>();
		agent = GetComponent<NavMeshAgent>();
		aiMovement = GetComponent<AIMovement>();
		commandListener = GetComponent<AiCommandListener>();
		entityVisibility = GetComponent<EntityVisibility>();
		shootingSystem = GetComponent<ShootingSystem>();
		abilitySystem = GetComponent<AbilitySystem>();
		abilitySystem.Initialize();
		health = GetComponent<Health>();
		aiDetection = GetComponent<AiDetectAndEngage>();
		aiDetection.Initialize();

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
		isControlledByPlayer = true;
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
		isControlledByPlayer = false;
	}

	public void HandleDeath()
	{
		//Previously didn't unsubscribe to the fire and shoot methods when dead. This is a temp fix.
		shootingSystem.DisableControl();
	}

	public void AddStatusEffect(IStatusEffect effect)
	{
		activeStatusEffects.Add(effect);
		effect.ApplyEffect(this);
		GameEvents.OnStatusEffectActivated?.Invoke(effect, this);
	}

	public void RemoveStatusEffect(IStatusEffect effect)
	{
		if (activeStatusEffects.Contains(effect))
		{
			effect.RemoveEffect(this);
			activeStatusEffects.Remove(effect);
			GameEvents.OnStatusEffectDeactivated?.Invoke(effect, this);
		}
	}

	public void ClearAllStatusEffects()
	{
		foreach (var effect in activeStatusEffects)
		{
			effect.RemoveEffect(this);
			GameEvents.OnStatusEffectDeactivated?.Invoke(effect, this);
		}

		activeStatusEffects.Clear();
	}

	public bool HasStatusEffect(IStatusEffect effect)
	{
		return activeStatusEffects.Contains(effect);
	}

}
