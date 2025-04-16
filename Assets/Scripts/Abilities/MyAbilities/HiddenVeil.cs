using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenVeil : MonoBehaviour
{

	private List<ShootingSystem> teammatesInVeil = new List<ShootingSystem>();
	private void OnTriggerEnter(Collider other)
	{
		
		if (other.gameObject.CompareTag("Teammate"))
		{
			EntityVisibility entityVisibility = other.gameObject.GetComponent<EntityVisibility>();
			entityVisibility.ChangeVisibilityModifier(0);
			ShootingSystem shootingSystem = other.gameObject.GetComponent<ShootingSystem>();


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
			EntityVisibility entityVisibility = other.gameObject.GetComponent<EntityVisibility>();
			entityVisibility.ChangeVisibilityModifier(1);

			ShootingSystem shootingSystem = other.gameObject.GetComponent<ShootingSystem>();

			
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
		//Script located on sphere inside of parent object
		Destroy(gameObject.transform.parent.gameObject);
	}
}
