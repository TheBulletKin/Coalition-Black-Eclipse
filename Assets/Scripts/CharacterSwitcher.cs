using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class CharacterSwitcher : MonoBehaviour
{

	[SerializeField] private List<ControllableEntity> teammates;
	public Camera playerCamera;
	public CameraController camController;
	public PlayerUiImanager uiManager;
	public CameraStateSwitcher cameraStateSwitcher;


	void Start()
	{
		InputManager.Instance.OnAiSwitchPressed += FindCharacter;
		playerCamera = Camera.main;
		

		FindCharacter(1);

	}

	//Checks to make sure that character exists before switching
	public void FindCharacter(int teammateId)
	{
		foreach (ControllableEntity teammate in teammates)
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

	private void SwitchToCharacter(ControllableEntity teammate)
	{

		foreach (ControllableEntity entity in teammates)
		{
			if (teammate.teammateID != entity.teammateID)
			{
				entity.LoseControl();
				entity.characterModel.SetActive(true);
				Debug.Log(entity.gameObject.name + " lost control");
			}
		}
		
		Debug.Log(teammate.gameObject.name + " gained control");
		MoveCamera(teammate.gameObject, teammate.cameraPos);
		teammate.characterModel.SetActive(false);
		uiManager.changePlayerTarget(teammate);
		cameraStateSwitcher.SwitchTarget(teammate);
			
		
		teammate.TakeControl();
	}

	private void MoveCamera(GameObject newOwner,Transform cameraPos)
	{
		playerCamera.transform.parent = cameraPos.parent;
		playerCamera.transform.localPosition = cameraPos.localPosition;
		camController.TransferOwnership(newOwner);
	}
}
