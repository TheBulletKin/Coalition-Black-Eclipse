using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUiImanager : MonoBehaviour, IToggleable, IInitialisable
{
	public static PlayerUiImanager Instance { get; private set; }

	[SerializeField] private ControllableEntity playerEntity;
	[SerializeField] private ControllableEntity playerEntityHolder;
	[SerializeField] private ControllableEntity commandedEntity;
	[SerializeField] private TextMeshProUGUI currentAmmoText;
	[SerializeField] private TextMeshProUGUI reserveAmmoText;
	//Shooting system assigned in CharacterSwitcher
	[SerializeField] private ShootingSystem shootingSystem;
	[SerializeField] private Crosshair crosshair;
	[SerializeField] private AbilitySystem abilitySystem;
	[SerializeField] private RectTransform abilityHotbarContainer;
	[SerializeField] private List<AbilityHotbarUiSlot> abilitySlots;
	[SerializeField] private CameraStateSwitcher cameraStateSwitcher;
	public Color hotbarSelectedColour;
	public int currentlySelectedHotbar;




	/// <summary>
	/// Requires: camera state switcher
	/// </summary>
	/// <returns></returns>
	public void Initialize()
	{		

		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		if (currentAmmoText == null || reserveAmmoText == null)
		{
			Debug.LogError("Assign all UI elements in the UIManager");
			
		}

		//GameEvents.OnGadgetActivated += UpdateAbilityHotbar;
		GameEvents.OnGadgetPlaced += UpdateAbilityHotbar;

		CameraStateSwitcher.OnCameraStateChanged += ChangeTargetBasedOnState;

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

	/// <summary>
	/// Change the UI elements to match the information of the new player
	/// </summary>
	/// <param name="newPlayer"></param>
	/// <param name="isTemp">When called after changing the teammate, this is true so it only updates the hotbar from the map view</param>
	public void changePlayerTarget(ControllableEntity newPlayer, bool isTemp)
	{
		if (isTemp) //To change hotbar in map view
		{
			playerEntityHolder = newPlayer;
		}
		else
		{
			playerEntity = newPlayer;
		}

		switch (cameraStateSwitcher.currentState)
		{
			case CameraStates.FPS:
				ChangeTarget(playerEntity, true);
				break;
			case CameraStates.TOPDOWN:
				if (playerEntityHolder != null)
				{
					ChangeTarget(playerEntityHolder, false);
				}
				break;
			default:
				break;
		}
	}

	private void ChangeTarget(ControllableEntity newEntity, bool flag)
	{
		if (flag)
		{
			if (abilitySystem)
			{
				abilitySystem.OnAbilitySelected -= SelectHotbarSlot;
			}

			
		}

		shootingSystem = newEntity.shootingSystem;
		abilitySystem = newEntity.abilitySystem;
		
		//If the target change is done to view abilities on other entities, keep it so that hotbar changes come from changes to the controlled character rather than the target
		if (flag)
		{
			shootingSystem.OnUpdateAmmo += UpdateAmmoCounts;
			shootingSystem.OnWeaponFired += UpdateAmmoCounts;

			abilitySystem.OnAbilitySelected += SelectHotbarSlot;
		}

		UpdateAbilityHotbar(null);
	}

	private void ChangeTargetBasedOnState(CameraStates cameraState)
	{
		switch (cameraState)
		{
			case CameraStates.FPS:
				ChangeTarget(playerEntity, true);
				break;
			case CameraStates.TOPDOWN:
				if (playerEntityHolder != null)
				{
					ChangeTarget(playerEntityHolder, false);
				}

				break;
			default:
				break;
		}
	}

	
}
