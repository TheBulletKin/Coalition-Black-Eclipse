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

	/* DEV NOTES FOR TONIGHT
	 * Unsure about how I will tackle individual ai movements and syncing. For now keep it simple
	 * Will consult design later, get something functional for now
	 * F1. F2 To select individual teammates. ` for all teammates
	 * Shift F1 F2 to select the whole group. Shift ` for all groups. 
	 * First add group selection to input manager, test.
	 * Each teammate needs a group index, assign manually for now.
	 * Select a team member and press execute key
	 * For now will do them individually. Figure out the rest after consolidating design
	 * Select teammate and select group handled here through events
	 * Changing group changes the index to the inputted value, sets groupSelection to true
	 * When executing commands, if this is true you can look at the group indexes.
	 * If not, just execute that teammate's commands.
	 * Change it so that execute command press just does the current group or teammate rather than them all
	 * Set it to 1 for the time being. These will simulate go codes later so this will be useful
	 * Will remove the synced command steps for now. Need to figure out design first. Come back to it later to reimplement
	 */



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
