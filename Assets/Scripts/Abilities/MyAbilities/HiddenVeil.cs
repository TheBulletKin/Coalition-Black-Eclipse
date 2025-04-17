using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class HiddenVeil : MonoBehaviour
{
	private HiddenStatusEffect hiddenStatusEffect = new HiddenStatusEffect();
	private List<ShootingSystem> teammatesInVeil = new List<ShootingSystem>();
	private Dictionary<ControllableEntity, HiddenStatusEffect> appliedEffects = new Dictionary<ControllableEntity, HiddenStatusEffect>();
	public float radius;

	private void Start()
	{

		//Used for when the veil is created and entities are already inside it
		Collider[] colliders = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Default"));

		foreach (Collider collider in colliders)
		{
			if (collider.gameObject.CompareTag("Teammate"))
			{
				if (collider.gameObject.GetComponent<ControllableEntity>()) //Check to confirm the object is a controllable entity
				{
					//This would be more efficient if it scanned with a layermask first rather than getting
					//  All these components and then deciding
					OnTriggerEnter(collider);
				}

			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{

		if (other.gameObject.CompareTag("Teammate"))
		{
			//Temporarily uses controllable entity as that holds status effects. Change this later for enemies
			ControllableEntity entity = other.GetComponent<ControllableEntity>();
			ShootingSystem shootingSystem = other.gameObject.GetComponent<ShootingSystem>();

			if (entity != null && !appliedEffects.ContainsKey(entity))
			{
				entity.AddStatusEffect(hiddenStatusEffect);
				appliedEffects.Add(entity, hiddenStatusEffect);
			}


			if (shootingSystem != null && !teammatesInVeil.Contains(shootingSystem))
			{
				shootingSystem.OnWeaponFired += ClearVeil;
				teammatesInVeil.Add(shootingSystem);
				Debug.Log(other.gameObject.name + " entered veil");
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{

		if (other.gameObject.CompareTag("Teammate"))
		{
			ControllableEntity entity = other.GetComponent<ControllableEntity>();
			ShootingSystem shootingSystem = other.GetComponent<ShootingSystem>();

			if (entity != null && appliedEffects.ContainsKey(entity))
			{
				entity.RemoveStatusEffect(appliedEffects[entity]);
				appliedEffects.Remove(entity);
			}


			if (shootingSystem != null && teammatesInVeil.Contains(shootingSystem))
			{
				shootingSystem.OnWeaponFired -= ClearVeil;
				teammatesInVeil.Remove(shootingSystem);
				Debug.Log(other.gameObject.name + " exited veil");
			}
		}
	}

	//Consider alternatives if passing in the shooting system proves to be a hassle
	//Destroy the veil if the entity inside shoots
	private void ClearVeil(ShootingSystem shootingSystem, int int1, int int2)
	{
		foreach (ShootingSystem teammate in teammatesInVeil)
		{
			teammate.OnWeaponFired -= ClearVeil;
		}

		List<ControllableEntity> entitiesToRemove = new List<ControllableEntity>(appliedEffects.Keys);

		foreach (ControllableEntity entity in entitiesToRemove)
		{
			entity.RemoveStatusEffect(appliedEffects[entity]);
			appliedEffects.Remove(entity);
		}

		Destroy(gameObject.transform.parent.gameObject);
	}

	public void SetStatusEffect(HiddenStatusEffect hiddenStatusEffect)
	{
		this.hiddenStatusEffect = hiddenStatusEffect;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 1f, 1f, 0.4f);
		Gizmos.DrawWireSphere(transform.position, radius);
	}


}
