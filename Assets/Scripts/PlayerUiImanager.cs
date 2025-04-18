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
	public Color hotbarSelectedColour;
	public int currentlySelectedHotbar;


	private void Start()
	{
		if (currentAmmoText == null || reserveAmmoText == null)
		{
			Debug.LogError("Assign all UI elements in the UIManager");
			return;
		}

		//GameEvents.OnGadgetActivated += UpdateAbilityHotbar;
		GameEvents.OnGadgetPlaced += UpdateAbilityHotbar;

		SelectHotbarSlot(0, 0);
	}

	private void Update()
	{
		crosshair.UpdateSpreadVisual(shootingSystem.currentBaseSpread);
	}

	private void SelectHotbarSlot(int newIndex, int oldIndex)
	{
		abilitySlots[oldIndex].ToggleHotbarSelection(false, hotbarSelectedColour);
		abilitySlots[oldIndex].isSelected = false;
		abilitySlots[newIndex].ToggleHotbarSelection(true, hotbarSelectedColour);
		abilitySlots[newIndex].isSelected = true;
		currentlySelectedHotbar = newIndex;

	}

	private void UpdateAbilityHotbar(IGadget gadget)
	{
		int currentIndex = 0;
		foreach (AbilityHotbarUiSlot slot in abilitySlots)
		{
			if (currentIndex < abilitySystem.abilities.Count && abilitySystem.abilities[currentIndex] != null)
			{
				slot.gameObject.SetActive(true);
				slot.ChangeHotbarSlotDetails(abilitySystem.abilities[currentIndex]);
				if (slot.isSelected)
				{
					SelectHotbarSlot(abilitySystem.currentAbilityIndex, currentlySelectedHotbar);
				}

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
		shootingSystem.OnUpdateAmmo -= UpdateAmmoCounts;
	}

	public void EnableControl()
	{

	}

	public void changePlayerTarget(ControllableEntity newPlayer)
	{
		playerEntity = newPlayer;
		shootingSystem = newPlayer.shootingSystem;
		shootingSystem.OnUpdateAmmo += UpdateAmmoCounts;
		shootingSystem.OnWeaponFired += UpdateAmmoCounts;

		//Ability system casts an event when a new ability is selected

		if (abilitySystem)
		{
			abilitySystem.OnAbilitySelected -= SelectHotbarSlot;
		}
		abilitySystem = newPlayer.abilitySystem;
		abilitySystem.OnAbilitySelected += SelectHotbarSlot;
		UpdateAbilityHotbar(null);


	}
}
