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

	public int currentTeammateIndex;

	public CameraStates cameraState;

		

	private void Start()
	{
		InputManager.Instance.OnInteractPressed += SingleMoveCommand;		
		//InputManager.Instance.OnCommandCreatePressed += QueueMoveCommand;
		CameraStateSwitcher.OnCameraStateChanged += UpdateCameraState;
		InputManager.Instance.OnQueueCommandPressed += QueueMoveCommand;
		InputManager.Instance.OnTeammateSelectPressed += ChangeCurrentTeammate;
		InputManager.Instance.OnAiExecuteCommandPressed += ExecuteInvidiualCommands;
		InputManager.Instance.OnAiExecuteTasksInSyncPressed += ExecuteCommandsSynchronously;

		//macroCommand = new MacroCommand();
		//macroCommand.OnMacroCompleted += OnMacroCompleted;

		//Set up the ai group, will be customisable later with more controls
		//Go through each teammate
		foreach (AiCommandListener teammate in aiTeammates)
		{
			//Add the other teammates aside from that one
			foreach (AiCommandListener otherTeammate in aiTeammates)
			{
				if (otherTeammate != teammate)
				{
					teammate.AddToGroup(otherTeammate);
				}
			}
		}
	}

	

	private void OnDestroy()
	{
		InputManager.Instance.OnInteractPressed -= SingleMoveCommand;		
		//InputManager.Instance.OnCommandCreatePressed -= QueueMoveCommand;
		CameraStateSwitcher.OnCameraStateChanged -= UpdateCameraState;
		InputManager.Instance.OnQueueCommandPressed -= QueueMoveCommand;
		InputManager.Instance.OnTeammateSelectPressed -= ChangeCurrentTeammate;
		InputManager.Instance.OnAiExecuteCommandPressed -= ExecuteInvidiualCommands;
		InputManager.Instance.OnAiExecuteTasksInSyncPressed -= ExecuteCommandsSynchronously;
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

	private void ExecuteInvidiualCommands(int AiIndex)
	{
		if (AiIndex == -1)
		{
			foreach (AiCommandListener entity in aiTeammates)
			{
				entity.RunCommand(false);
			}
		}
		else if (AiIndex < aiTeammates.Count) 
		{
			aiTeammates[AiIndex].RunCommand(false);
		}
	}

	private void ExecuteCommandsSynchronously()
	{
		/* Need to tell each Ai when the other finishes
		 * Set up an ai group.
		 * For now will be really simple, will figure it out as I come up with more control options and designs
		 * Make it so that everyone is in a group, so one listener holds the others
		 * Hold how many AIs there are as an int. That's just .Count
		 * Run the method on each group member
		 */

		//Each ai team member has an ai group attached to them.
		//The RunSyncedCommand method will start running the commands on all ai in that teammate's group
		//Don't like how this works
		aiTeammates[0].RunSyncedCommand();
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
		IssueImmediateCommand(newMoveCommand);
	}

	/// <summary>
	/// Add a move command to the macro queue
	/// </summary>
	public void QueueMoveCommand()
	{
		Vector3 TargetPostion = TryGetSelectedPosition();
		if (TargetPostion != Vector3.zero)
		{
			//macroCommand.AddCommand(CreateMoveCommand(TargetPostion));
			aiTeammates[currentTeammateIndex].AddCommand(CreateMoveCommand(TargetPostion));
		}
		
	}

	

	


	

	/// <summary>
	/// Used to instantly perform a command. Will be reintroduced later
	/// </summary>
	/// <param name="command"></param>
	private void IssueImmediateCommand(ICommand command)
	{
		command.Execute();
	}

	private void UpdateCameraState(CameraStates state)
	{
		cameraState = state;
	}
}
