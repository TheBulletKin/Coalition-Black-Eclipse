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
	[SerializeField] private Crosshair crosshair;
	[SerializeField] private AbilitySystem abilitySystem;
	[SerializeField] private RectTransform abilityHotbarContainer;
	[SerializeField] private List<AbilityHotbarUiSlot> abilitySlots;


	private void Start()
	{
		if (currentAmmoText == null || reserveAmmoText == null)
		{
			Debug.LogError("Assign all UI elements in the UIManager");
			return;
		}

	}

	private void Update()
	{
		crosshair.UpdateSpreadVisual(shootingSystem.currentBaseSpread);
	}

	private void UpdateAbilityHotbar()
	{
		int currentIndex = 0;
		foreach (AbilityHotbarUiSlot slot in abilitySlots)
		{
			if (currentIndex < abilitySystem.abilities.Count && abilitySystem.abilities[currentIndex] != null)
			{
				slot.gameObject.SetActive(true);
				slot.ChangeHotbarSlotDetails(abilitySystem.abilities[currentIndex]);
			}
			else //When no ability exists for the current index
			{
				slot.gameObject.SetActive(false);
			}

			currentIndex++;
		}

	}

	private void UpdateAmmoCounts(ShootingSystem shootingSystem, int currentAmmo, int reserveAmmo)
	{
		if (shootingSystem.inPlayerControl)
		{
			currentAmmoText.text = currentAmmo.ToString();
			reserveAmmoText.text = reserveAmmo.ToString();
		}


	}

	public void DisableControl()
	{
		shootingSystem.OnWeaponFired -= UpdateAmmoCounts;
	}

	public void EnableControl()
	{

	}

	public void changePlayerTarget(ControllableEntity newPlayer)
	{
		playerEntity = newPlayer;
		shootingSystem = newPlayer.gameObject.GetComponent<ShootingSystem>();
		shootingSystem.OnWeaponFired += UpdateAmmoCounts;

		abilitySystem = newPlayer.gameObject.GetComponent<AbilitySystem>();
		UpdateAbilityHotbar();

	}
}
