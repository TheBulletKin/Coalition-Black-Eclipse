using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCommandIssuer : MonoBehaviour
{
	public List<AiEntity> aiTeammates;
	public float interactionRange = 3f;
	public LayerMask interactableLayer;

	public int currentTeammateIndex;

	public CameraStates cameraState;

	//A macro command is a series of sub commands.
	public MacroCommand macroCommand;	

	private void Start()
	{
		InputManager.Instance.OnInteractPressed += SingleMoveCommand;
		InputManager.Instance.OnExecutePressed += ExecuteMacroCommand;
		//InputManager.Instance.OnCommandCreatePressed += QueueMoveCommand;
		CameraStateSwitcher.OnCameraStateChanged += UpdateCameraState;
		InputManager.Instance.OnQueueCommandPressed += QueueMoveCommand;
		InputManager.Instance.OnTeammateSelectPressed += ChangeCurrentTeammate;

		macroCommand = new MacroCommand();
		macroCommand.OnMacroCompleted += OnMacroCompleted;
	}

	

	private void OnDestroy()
	{
		InputManager.Instance.OnInteractPressed -= SingleMoveCommand;
		InputManager.Instance.OnExecutePressed -= ExecuteMacroCommand;
		//InputManager.Instance.OnCommandCreatePressed -= QueueMoveCommand;
		CameraStateSwitcher.OnCameraStateChanged -= UpdateCameraState;
		InputManager.Instance.OnQueueCommandPressed -= QueueMoveCommand;
		InputManager.Instance.OnTeammateSelectPressed -= ChangeCurrentTeammate;
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

	private void ChangeCurrentTeammate(int teammateIndex)
	{
		currentTeammateIndex = teammateIndex;
		Debug.Log("Teammate selected: " + teammateIndex);
	}

	/// <summary>
	/// Try to place a command where the crosshair is, currently just move commands
	/// </summary>
	private ICommand CreateMoveCommand(Vector3 targetPosition)
	{
		ICommand newMoveCommand = new MoveCommand(aiTeammates[currentTeammateIndex].GetComponent<AIMovement>(), targetPosition);
		return newMoveCommand;
	}

	/// <summary>
	/// Create a move command and add it to the macroCommand
	/// </summary>
	/// <param name="targetPosition"></param>
	public void SingleMoveCommand()
	{
		Vector3 targetPosition = TryGetSelectedPosition();
		ICommand newMoveCommand = new MoveCommand(aiTeammates[currentTeammateIndex].GetComponent<AIMovement>(), targetPosition);
		IssueCommand(newMoveCommand);
	}

	/// <summary>
	/// Add a move command to the macro queue
	/// </summary>
	public void QueueMoveCommand()
	{
		Vector3 TargetPostion = TryGetSelectedPosition();
		if (TargetPostion != Vector3.zero)
		{
			macroCommand.AddCommand(CreateMoveCommand(TargetPostion));
		}
		
	}


	/// <summary>
	/// Ran when the user presses the 'execute' keybind
	/// </summary>
	public void ExecuteMacroCommand()
	{
		macroCommand.Execute();
	}

	/// <summary>
	/// Ran when the internal macro keybind finishes its tasks
	/// </summary>
	private void OnMacroCompleted()
	{
		macroCommand.OnMacroCompleted -= OnMacroCompleted;
		Debug.Log("Macro command completed");
	}

	/// <summary>
	/// Used to instantly perform a command. Will be reintroduced later
	/// </summary>
	/// <param name="command"></param>
	private void IssueCommand(ICommand command)
	{
		command.Execute();
	}

	private void UpdateCameraState(CameraStates state)
	{
		cameraState = state;
	}
}
