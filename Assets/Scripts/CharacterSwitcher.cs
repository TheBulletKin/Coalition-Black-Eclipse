using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class CharacterSwitcher : MonoBehaviour, IInitialisable
{
	public static CharacterSwitcher Instance { get; private set; }

	public Camera playerCamera;
	public CameraController camController;
	public PlayerUiImanager playerUiManager;
	public GadgetUiManager gadgetUiManager;
	public CameraStateSwitcher cameraStateSwitcher;
	public int currentlyControlledTeammate;

	

	/// <summary>
	/// Requires: InputManager, PlayerUiManager, gadgetUiManager, cameraStateSwitcher
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

		InputManager.Instance.OnAiSwitchPressed += FindCharacter;
		playerCamera = Camera.main;

		FindCharacter(1);


	}


	//Checks to make sure that character exists before switching
	public void FindCharacter(int teammateId)
	{
		foreach (ControllableEntity teammate in EntityManager.Instance.playerTeammates)
		{
			if (teammate.teammateID == teammateId)
			{
				/* Disable current player control related components
				 * Enable ai control components
				 * Enable current model
				 * Disable target's model
				 * 
				 */

				SwitchToCharacter(teammate);
				
			}
		}
	}

	public void SwitchToCharacter(ControllableEntity teammate)
	{
		//First, disable player control on all entities
		foreach (ControllableEntity entity in EntityManager.Instance.playerTeammates)
		{
			if (teammate.teammateID != entity.teammateID)
			{
				entity.LoseControl();
				entity.characterModel.SetActive(true);
				entity.isControlledByPlayer = false;
				//Debug.Log(entity.gameObject.name + " lost control");
			}
		}
		
		//Debug.Log(teammate.gameObject.name + " gained control");

		//Move the player camera and change parent
		MoveCamera(teammate.gameObject, teammate.cameraPos);

		//Disable the player model on the selected character
		teammate.characterModel.SetActive(false);

		//Change ammo UI to match selected player
		playerUiManager.changePlayerTarget(teammate, false);

		//Change status effects and gadets to match selected player
		gadgetUiManager.ChangeVisibleUiElements(teammate);

		//Change the target for map / player view switching
		cameraStateSwitcher.SwitchTarget(teammate);

		teammate.aiMovement.EnableControl();
		
		teammate.TakeControl();

		teammate.isControlledByPlayer = true;

		currentlyControlledTeammate = teammate.teammateID;
	}

	private void MoveCamera(GameObject newOwner,Transform cameraPos)
	{
		playerCamera.transform.parent = cameraPos.parent;
		playerCamera.transform.localPosition = cameraPos.localPosition;
		camController.TransferOwnership(newOwner);
		camController.AimInDirection(newOwner.transform.forward);
	}
}
