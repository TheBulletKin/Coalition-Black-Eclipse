using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingSystem : MonoBehaviour, IToggleable
{
	Camera mainCam;

	[SerializeField] LayerMask hittableLayers;

	[SerializeField] public WeaponConfig weaponConfig;
	[SerializeField] public int currentAmmo;
	[SerializeField] public int reserveAmmo;
	private float fireTimer = 0;
	private bool isReloading = false;
	private bool isFireRecovery = false;
	private bool inPlayerControl = false;

	public event Action<int, int> WeaponFired;
	

	void Start()
	{
		mainCam = Camera.main;
		currentAmmo = weaponConfig.maxAmmo;
		inPlayerControl = false;
		
	}

	// Update is called once per frame
	void Update()
	{
		if (isFireRecovery)
		{
			fireTimer += Time.deltaTime;
			if (fireTimer >= 1 / (weaponConfig.fireRate / 60))
			{
				isFireRecovery = false;
				fireTimer = 0;
			}
		}

	}

	private void Fire()
	{
		if (isReloading || currentAmmo <= 0 || isFireRecovery) return;

		Ray fireRay = mainCam.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit hit, weaponConfig.weaponRange, hittableLayers))
		{
			IDamagable damageable = hit.collider.GetComponent<IDamagable>();
			if (damageable != null)
			{
				damageable.TakeDamage(weaponConfig.weaponDamage);
			}
		}

		isFireRecovery = true;
		
		currentAmmo--;
		if (currentAmmo <= 0)
		{
			Reload();
		}

		WeaponFired.Invoke(currentAmmo, reserveAmmo);
	}

	private void Reload()
	{
		if (isReloading)
		{
			return;
		}
		else if (currentAmmo != weaponConfig.maxAmmo)
		{
			isReloading = true;
		}
				

		StartCoroutine(ReloadRoutine());

	}

	private IEnumerator ReloadRoutine()
	{
		yield return new WaitForSeconds(1.5f);

		if (reserveAmmo > 0)
		{
			
			if (reserveAmmo >= weaponConfig.maxAmmo)
			{
				reserveAmmo -= weaponConfig.maxAmmo - currentAmmo;
				currentAmmo = weaponConfig.maxAmmo;
				UpdateAmmo(currentAmmo, reserveAmmo);
			}
			else
			{
				
				currentAmmo = reserveAmmo;
				reserveAmmo = 0;
				UpdateAmmo(currentAmmo, reserveAmmo);
			}
		}
		isReloading = false;
		
	}

	public void UpdateAmmo(int currentAmmo, int reserveAmmo)
	{
		WeaponFired.Invoke(currentAmmo, reserveAmmo);
	}

	public void DisableControl()
	{
		inPlayerControl = false;
		InputManager.Instance.OnFirePressed += Fire;
		InputManager.Instance.OnReloadPressed += Reload;
	}

	public void EnableControl()
	{
		inPlayerControl = true;
		InputManager.Instance.OnFirePressed -= Fire;
		InputManager.Instance.OnReloadPressed -= Reload;
	}
}
