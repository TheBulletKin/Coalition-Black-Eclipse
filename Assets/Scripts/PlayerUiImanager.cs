using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUiImanager : MonoBehaviour
{
	[SerializeField] private PlayerEntity playerEntity;
    [SerializeField] private TextMeshProUGUI currentAmmoText;
	[SerializeField] private TextMeshProUGUI reserveAmmoText;
	[SerializeField] private ShootingSystem shootingSystem;

	private void Start()
	{
		if (currentAmmoText == null || reserveAmmoText == null)
		{
			Debug.LogError("Assign all UI elements in the UIManager");
			return;
		}
		shootingSystem = playerEntity.GetComponent<ShootingSystem>();
		shootingSystem.WeaponFired += UpdateAmmoCounts;
	}

	private void UpdateAmmoCounts(int currentAmmo, int reserveAmmo)
	{
		currentAmmoText.text = currentAmmo.ToString();
		reserveAmmoText.text = reserveAmmo.ToString();
	}

	
}
