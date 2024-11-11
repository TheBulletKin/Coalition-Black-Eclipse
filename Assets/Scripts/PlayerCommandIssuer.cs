using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCommandIssuer : MonoBehaviour
{
	public AIMovement aiMovement;
	public float interactionRange = 3f;
	public LayerMask interactableLayer;

	public CameraStates cameraState;

	//A macro command is a series of sub commands.
	public MacroCommand macroCommand;
	public MoveCommand newMoveCommand;

	private void Start()
	{
		InputManager.Instance.OnInteractPressed += CommandMove;
		InputManager.Instance.OnExecutePressed += ExecuteMacroCommand;
		InputManager.Instance.OnCommandCreatePressed += TryMoveCommand;
		CameraStateSwitcher.OnCameraStateChanged += UpdateCameraState;
		InputManager.Instance.OnQueueCommandPressed += QueueCommand;

		macroCommand = new MacroCommand();
		macroCommand.OnMacroCompleted += OnMacroCompleted;
	}	

	private void OnDestroy()
	{
		InputManager.Instance.OnInteractPressed -= CommandMove;
		InputManager.Instance.OnExecutePressed -= ExecuteMacroCommand;
		InputManager.Instance.OnCommandCreatePressed -= TryMoveCommand;
		CameraStateSwitcher.OnCameraStateChanged -= UpdateCameraState;
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
				return null;
				break;
		}
	}

	/// <summary>
	/// Try to place a command where the crosshair is, currently just move commands
	/// </summary>
	private void TryMoveCommand()
	{
		switch (cameraState)
		{
			case CameraStates.FPS:
				Ray firstPersonray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
				RaycastHit hit;

				if (Physics.Raycast(firstPersonray, out hit, interactionRange, interactableLayer))
				{					
					newMoveCommand = new MoveCommand(aiMovement, hit.point);
					
				}
				break;
			case CameraStates.TOPDOWN:
				Ray topDownRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
				RaycastHit topDownHit;

				if (Physics.Raycast(topDownRay, out topDownHit, interactionRange, interactableLayer))
				{
					newMoveCommand = new MoveCommand(aiMovement, topDownHit.point);
					
				}
				break;
			default:				
				break;
		}
		

	}

	/// <summary>
	/// Create a move command and add it to the macroCommand
	/// </summary>
	/// <param name="targetPosition"></param>
	public void CommandMove(Vector3 targetPosition)
	{
		newMoveCommand = new MoveCommand(aiMovement, targetPosition);
		IssueCommand(newMoveCommand);
	}

	public void QueueCommand()
	{
		TryMoveCommand();
		macroCommand.AddCommand(newMoveCommand);
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
