using System;
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

	private void Start()
	{
		InputManager.Instance.OnInteractPressed += TryCommand;
		InputManager.Instance.OnExecutePressed += ExecuteCommands;
		InputManager.Instance.OnCommandCreatePressed += TryCommand;
		CameraStateSwitcher.OnCameraStateChanged += UpdateCameraState;

		macroCommand = new MacroCommand();
		macroCommand.OnMacroCompleted += OnMacroCompleted;
	}

	

	private void OnDestroy()
	{
		InputManager.Instance.OnInteractPressed -= TryCommand;
		InputManager.Instance.OnExecutePressed -= ExecuteCommands;
		InputManager.Instance.OnCommandCreatePressed -= TryCommand;
		CameraStateSwitcher.OnCameraStateChanged -= UpdateCameraState;
	}

	/// <summary>
	/// Try to place a command where the crosshair is, currently just move commands
	/// </summary>
	private void TryCommand()
	{
		switch (cameraState)
		{
			case CameraStates.FPS:
				Ray firstPersonray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
				RaycastHit hit;

				if (Physics.Raycast(firstPersonray, out hit, interactionRange, interactableLayer))
				{
					CommandMove(hit.point);
				}
				break;
			case CameraStates.TOPDOWN:
				Ray topDownRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
				RaycastHit topDownHit;

				if (Physics.Raycast(topDownRay, out topDownHit, interactionRange, interactableLayer))
				{
					CommandMove(topDownHit.point); 
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
		ICommand moveCommand = new MoveCommand(aiMovement, targetPosition);
		macroCommand.AddCommand(moveCommand);
	}


	/// <summary>
	/// Ran when the user presses the 'execute' keybind
	/// </summary>
	public void ExecuteCommands()
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
