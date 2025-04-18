using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HiddenVeilAbility", menuName = "Abilities/Hidden Veil")]
public class HiddenVeilAbility : CharacterAbility
{	
	[SerializeField] private float placementRange = 20f;
	[SerializeField] private float veilRadius = 5f;
	[SerializeField] private GameObject proxSensorPrefab;
	[SerializeField] private List<HiddenVeil> activeVeils;
	[HideInInspector] private HiddenStatusEffect hiddenStatusEffect;

	public override void Init(AbilitySystem owner)
	{
		base.Init(owner);
		activeVeils = new List<HiddenVeil>();
		hiddenStatusEffect = new HiddenStatusEffect();
	}

	public override void Use(AbilitySystem owner)
	{

	}

	public override void Use(AbilitySystem owner, GameObject target)
	{

	}

	public override void Use(AbilitySystem owner, RaycastHit targetPos)
	{

	}

	public override void Use(AbilitySystem owner, Vector3 targetVecPos)
	{

		if (Vector3.Distance(owner.transform.position, targetVecPos) <= placementRange && currentAbilityCount > 0)
		{
			GameObject newVeilObject = Instantiate(proxSensorPrefab, targetVecPos, Quaternion.identity);
			HiddenVeil hiddenVeil = newVeilObject.GetComponentInChildren<HiddenVeil>();
			
			hiddenVeil.radius = veilRadius;
			hiddenVeil.SetVeilEnterCallback(OnEnterVeil);
			hiddenVeil.SetVeilExitCallback(OnExitVeil);

			currentAbilityCount--;

			GameEvents.OnGadgetPlaced?.Invoke(hiddenVeil);
		}

	}

	/// <summary>
	/// Called by the hidden veil object when an entity enters it
	/// </summary>
	/// <param name="entity"></param>
	public void OnEnterVeil(ControllableEntity entity, HiddenVeil veil)
	{
		if (!entity.entityVisibility.activeVeils.Contains(veil))
		{
			entity.entityVisibility.activeVeils.Add(veil);
		}
		
		if (!entity.HasStatusEffect(hiddenStatusEffect))
		{
			entity.AddStatusEffect(hiddenStatusEffect);			
		}			
	}

	/// <summary>
	/// Called by the hidden veil when an entity leaves it
	/// </summary>
	/// <param name="entity"></param>
	public void OnExitVeil(ControllableEntity entity, HiddenVeil veil)
	{
		entity.entityVisibility.activeVeils.Remove(veil);
		if (entity.HasStatusEffect(hiddenStatusEffect) && entity.entityVisibility.activeVeils.Count == 0)
		{
			entity.RemoveStatusEffect(hiddenStatusEffect);
		}
	}
}
