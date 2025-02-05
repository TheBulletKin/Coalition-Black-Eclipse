using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AmmoPickup : MonoBehaviour, IInteractable
{
	
	public void Interact(GameObject instigator)
	{
		ShootingSystem playerShootingSystem = instigator.GetComponent<ShootingSystem>();
		playerShootingSystem.UpdateAmmo(playerShootingSystem.currentAmmo, playerShootingSystem.reserveAmmo += (int)(playerShootingSystem.weaponConfig.maxAmmo * 0.5));
		Destroy(this);
	}
}
