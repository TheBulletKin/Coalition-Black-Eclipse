using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUiImanager : MonoBehaviour, IToggleable
{
	[SerializeField] private ControllableEntity playerEntity;
    [SerializeField] private TextMeshProUGUI currentAmmoText;
	[SerializeField] private TextMeshProUGUI reserveAmmoText;
	//Shooting system assigned in CharacterSwitcher
	[SerializeField] private ShootingSystem shootingSystem;

	private void Start()
	{
		if (currentAmmoText == null || reserveAmmoText == null)
		{
			Debug.LogError("Assign all UI elements in the UIManager");
			return;
		}
		
		
	}

	private void UpdateAmmoCounts(int currentAmmo, int reserveAmmo)
	{
		currentAmmoText.text = currentAmmo.ToString();
		reserveAmmoText.text = reserveAmmo.ToString();
	}

	public void DisableControl()
	{
		shootingSystem.WeaponFired -= UpdateAmmoCounts;
	}

	public void EnableControl()
	{
		
	}

	public void changePlayerTarget(ControllableEntity newPlayer)
	{
		playerEntity = newPlayer;
		shootingSystem = newPlayer.gameObject.GetComponent<ShootingSystem>();
		shootingSystem.WeaponFired += UpdateAmmoCounts;
		
	}
}
