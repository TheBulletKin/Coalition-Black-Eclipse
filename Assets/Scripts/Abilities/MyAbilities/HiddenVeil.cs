using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class HiddenVeil : MonoBehaviour, IGadget
{
	private HiddenStatusEffect hiddenStatusEffect = new HiddenStatusEffect();
	//Hold the entities currently in the veil so that shooting can break it
	[SerializeField] private List<ShootingSystem> teammatesInVeil = new List<ShootingSystem>();
	public float radius;
	
	private Action<ControllableEntity, HiddenVeil> OnVeilEnter;
	private Action<ControllableEntity, HiddenVeil> OnVeilExit;

	public Transform GadgetTransform => transform;


	/* Hidden veil made up of two parts. This is the physical object, the other is the ability script itself
	 * This component is placed on the in world hidden veil. It manages responding to entities entering and exiting the veil.
	 * Uses Actions since the response to entities exiting and entering the veil is handled in the HiddenVeilAbility class
	 */

	private void Start()
	{
		//Used for when the veil is created and entities are already inside it
		Collider[] colliders = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Default"));

		foreach (Collider collider in colliders)
		{
			if (collider.gameObject.CompareTag("Teammate"))
			{
				ControllableEntity entity = collider.gameObject.GetComponent<ControllableEntity>();
				if (entity) 
				{ 					
					OnVeilEnter?.Invoke(entity, this);
				}

			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{

		if (other.gameObject.CompareTag("Teammate"))
		{
			//Temporarily uses controllable entity as that holds status effects. Change this later for enemies
			ControllableEntity entity = other.gameObject.GetComponent<ControllableEntity>();
			if (entity)
			{
				OnVeilEnter?.Invoke(entity, this);
			}


			ShootingSystem shootingSystem = other.gameObject.GetComponent<ShootingSystem>();

			if (shootingSystem != null && !teammatesInVeil.Contains(shootingSystem))
			{
				//Uses the shooting system's weapon fired event to clear this veil for simplicty sake
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
			ControllableEntity entity = other.gameObject.GetComponent<ControllableEntity>();
			if (entity)
			{
				OnVeilExit?.Invoke(entity, this);
			}

			ShootingSystem shootingSystem = other.GetComponent<ShootingSystem>();
				
			if (shootingSystem != null && teammatesInVeil.Contains(shootingSystem))
			{
				shootingSystem.OnWeaponFired -= ClearVeil;
				teammatesInVeil.Remove(shootingSystem);
				Debug.Log(other.gameObject.name + " exited veil");
			}
		}
	}

	
	//Destroy the veil if the entity inside shoots
	private void ClearVeil(ShootingSystem shootingSystem, int int1, int int2)
	{
		foreach (ShootingSystem teammate in teammatesInVeil)
		{
			teammate.OnWeaponFired -= ClearVeil;
			
			ControllableEntity entity = teammate.GetComponent<ControllableEntity>();
			if (entity != null)
			{
				OnVeilExit?.Invoke(entity, this);
			}
		}
		Destroy(gameObject.transform.parent.gameObject);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 1f, 1f, 0.4f);
		Gizmos.DrawWireSphere(transform.position, radius);
	}

	public void SetVeilEnterCallback(Action<ControllableEntity, HiddenVeil> callback)
	{
		OnVeilEnter = callback;
	}

	public void SetVeilExitCallback(Action<ControllableEntity, HiddenVeil> callback)
	{
		OnVeilExit = callback;
	}


}
