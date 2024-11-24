using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class responsible for assigning commands to Ai teammates and telling them when to begin executing tasks and how.
/// </summary>
public class PlayerCommandIssuer : MonoBehaviour
{
	public List<AiCommandListener> aiTeammates;
	public float interactionRange = 3f;
	public LayerMask interactableLayer;
	public int currentGroupOrTeammateIndex;
	public bool selectingGroup;

	public CameraStates cameraState;

	private void Start()
	{
		InputManager.Instance.OnInteractPressed += InstantKeyPressed;		
		CameraStateSwitcher.OnCameraStateChanged += UpdateCameraState;
		InputManager.Instance.OnQueueCommandPressed += QueueKeyPressed;
		InputManager.Instance.OnTeammateSelectPressed += ChangeCurrentTeammate;
		InputManager.Instance.OnGoCodePressed += ExecuteCommands;
		InputManager.Instance.OnAiGroupSelectedPressed += ChangeCurrentGroup;
		InputManager.Instance.OnLookCommandInstantPressed += InstantLook;
		InputManager.Instance.OnLookCommandQueuePressed += QueueLook;
	}

	

	private void OnDestroy()
	{
		InputManager.Instance.OnInteractPressed -= InstantKeyPressed;		
		CameraStateSwitcher.OnCameraStateChanged -= UpdateCameraState;
		InputManager.Instance.OnQueueCommandPressed -= QueueKeyPressed;
		InputManager.Instance.OnTeammateSelectPressed -= ChangeCurrentTeammate;
		InputManager.Instance.OnGoCodePressed -= ExecuteCommands;
		InputManager.Instance.OnAiGroupSelectedPressed -= ChangeCurrentGroup;
	}

	private Vector3 TryGetSelectedPosition()
	{
		switch (cameraState)
		{
			case CameraStates.FPS:
				Ray firstPersonray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
				RaycastHit hit;

				if (Physics.Raycast(firstPersonray, out hit, interactionRange, interactableLayer))
				{
					return hit.point;
				}
				break;
			case CameraStates.TOPDOWN:
				Ray topDownRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
				RaycastHit topDownHit;

				if (Physics.Raycast(topDownRay, out topDownHit, interactionRange, interactableLayer))
				{
					return topDownHit.point;
				}
				break;
			default:

				break;
		}
		return Vector3.zero;
	}

	private void ChangeCurrentGroup(int groupIndex)
	{
		currentGroupOrTeammateIndex = groupIndex;
		selectingGroup = true;
		Debug.Log("Group selected: " + groupIndex);
	}

	private void ChangeCurrentTeammate(int teammateIndex)
	{
		currentGroupOrTeammateIndex = teammateIndex;
		selectingGroup = false;
		Debug.Log("Teammate selected: " + teammateIndex);
	}

	private void ExecuteCommands()
	{
		if (currentGroupOrTeammateIndex == -1)
		{
			foreach (AiCommandListener teammate in aiTeammates)
			{
				teammate.RunCommand();
			}
		}
		else if (selectingGroup)
		{
			foreach (AiCommandListener teammate in aiTeammates)
			{
				if (teammate.groupIndex == currentGroupOrTeammateIndex)
				{
					teammate.RunCommand();
				}
			}
		}
		else
		{
			aiTeammates[currentGroupOrTeammateIndex].RunCommand();
		}
	}



	private ICommand CreateLookCommand(AiCommandListener targetAi, Vector3 targetPosition)
	{
		ICommand newLookCommand = new LookCommand(targetAi.GetComponent<AIMovement>(), targetPosition);
		return newLookCommand;
	}

	private void InstantLook()
	{
		QueueOrPerformLookCommand(false);
	}

	private void QueueLook()
	{
		QueueOrPerformLookCommand(true);
	}

	/// <summary>
	/// Try to place a command where the crosshair is, currently just move commands
	/// </summary>
	private ICommand CreateMoveCommand(AiCommandListener targetAi, Vector3 targetPosition)
	{
		ICommand newMoveCommand = new MoveCommand(targetAi.GetComponent<AIMovement>(), targetPosition);
		return newMoveCommand;
	}	

	public void QueueKeyPressed()
	{
		QueueOrPerformMoveCommand(true);
	}

	public void InstantKeyPressed()
	{
		QueueOrPerformMoveCommand(false);
	}

	public void QueueOrPerformLookCommand(bool shouldQueue)
	{
		Vector3 targetPosition = TryGetSelectedPosition();

		if (selectingGroup)
		{
			foreach (AiCommandListener teammate in aiTeammates)
			{
				if (teammate.groupIndex == currentGroupOrTeammateIndex)
				{
					if (shouldQueue)
					{
						teammate.AddCommand(CreateLookCommand(teammate, targetPosition));
					}
					else
					{
						teammate.RunCommand(CreateLookCommand(teammate, targetPosition));
					}

				}
			}
		}
		else
		{
			if (shouldQueue)
			{
				aiTeammates[currentGroupOrTeammateIndex].AddCommand(CreateLookCommand(aiTeammates[currentGroupOrTeammateIndex], targetPosition));
			}
			else
			{
				aiTeammates[currentGroupOrTeammateIndex].RunCommand(CreateLookCommand(aiTeammates[currentGroupOrTeammateIndex], targetPosition));
			}
		}
	}
	

	public void QueueOrPerformMoveCommand(bool shouldQueue)
	{
		Vector3 targetPosition = TryGetSelectedPosition();

		if (selectingGroup)
		{
			foreach (AiCommandListener teammate in aiTeammates)
			{
				if (teammate.groupIndex == currentGroupOrTeammateIndex)
				{
					if (shouldQueue)
					{
						teammate.AddCommand(CreateMoveCommand(teammate, targetPosition));
					}
					else
					{
						teammate.RunCommand(CreateMoveCommand(teammate, targetPosition));
					}

				}
			}
		}
		else
		{
			if (shouldQueue)
			{
				aiTeammates[currentGroupOrTeammateIndex].AddCommand(CreateMoveCommand(aiTeammates[currentGroupOrTeammateIndex], targetPosition));
			}
			else
			{
				aiTeammates[currentGroupOrTeammateIndex].RunCommand(CreateMoveCommand(aiTeammates[currentGroupOrTeammateIndex], targetPosition));
			}
		}
	}


	private void UpdateCameraState(CameraStates state)
	{
		cameraState = state;
	}
}
