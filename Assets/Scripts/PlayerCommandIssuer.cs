using System.Collections.Generic;
using System.Drawing;
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
	public GameObject moveMarker;
	public GameObject lookMarker;



	public CameraStates cameraState;

	private void Start()
	{
		CameraStateSwitcher.OnCameraStateChanged += UpdateCameraState;

		//Command Creation
		InputManager.Instance.OnCommandCreatePressed += CreateCommandPressed;

		//Teammate and Group selection
		InputManager.Instance.OnTeammateSelectPressed += ChangeCurrentTeammate;
		InputManager.Instance.OnAiGroupSelectedPressed += ChangeCurrentGroup;
		//Go codes
		InputManager.Instance.OnGoCodePressed += ExecuteCommands;


	}


	private void OnDestroy()
	{
		CameraStateSwitcher.OnCameraStateChanged -= UpdateCameraState;

		//Command Creation
		InputManager.Instance.OnCommandCreatePressed -= CreateCommandPressed;

		//Teammate and Group selection
		InputManager.Instance.OnTeammateSelectPressed -= ChangeCurrentTeammate;
		InputManager.Instance.OnAiGroupSelectedPressed -= ChangeCurrentGroup;
		//Go codes
		InputManager.Instance.OnGoCodePressed -= ExecuteCommands;
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

	private void ExecuteCommands(int goCode)
	{
		//goCode unused for the moment
		//Currently just executes commands for current teammate or all teammates in group
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



	//---- Command Creation	

	public void CreateCommandPressed(CommandType commandType, bool shouldQueue)
	{
		if (selectingGroup)
		{
			foreach (AiCommandListener teammate in aiTeammates)
			{
				if (teammate.groupIndex == currentGroupOrTeammateIndex)
				{
					if (shouldQueue)
					{
						teammate.AddCommand(CreateCommandFromType(teammate, commandType));
						CreateCommandWaypoint(teammate, commandType);
					}
					else
					{
						teammate.RunCommand(CreateCommandFromType(teammate, commandType));
					}

				}
			}
		}
		else
		{
			if (shouldQueue)
			{
				aiTeammates[currentGroupOrTeammateIndex].AddCommand(CreateCommandFromType(aiTeammates[currentGroupOrTeammateIndex], commandType));
				CreateCommandWaypoint(aiTeammates[currentGroupOrTeammateIndex], commandType);
			}
			else
			{
				aiTeammates[currentGroupOrTeammateIndex].RunCommand(CreateCommandFromType(aiTeammates[currentGroupOrTeammateIndex], commandType));
			}
		}
	}

	private ICommand CreateCommandFromType(AiCommandListener entity, CommandType commandType)
	{
		ICommand newCommand = null;
		Vector3 targetPosition = Vector3.zero;
		switch (commandType)
		{
			case CommandType.MOVE:
				targetPosition = TryGetSelectedPosition();
				newCommand = new MoveCommand(entity.GetComponent<AIMovement>(), targetPosition);
				return newCommand;
			case CommandType.LOOK:
				targetPosition = TryGetSelectedPosition();
				newCommand = new LookCommand(entity.GetComponent<AIMovement>(), targetPosition);
				return newCommand;
			case CommandType.NONE:
				return null;
			default:
				return null;
		}
	}

	private void CreateCommandWaypoint(AiCommandListener teammate, CommandType commandType)
	{
		Vector3 targetPosition = TryGetSelectedPosition();
		targetPosition = new Vector3(targetPosition.x, targetPosition.y + 0.2f, targetPosition.z);

		GameObject newWaypoint = null;

		switch (commandType)
		{
			case CommandType.MOVE:
				newWaypoint = Instantiate(moveMarker, targetPosition, Quaternion.identity);
				break;
			case CommandType.LOOK:
				newWaypoint = Instantiate(lookMarker, targetPosition, Quaternion.identity);
				break;
			case CommandType.NONE:
				break;
			default:
				break;
		}

		if (newWaypoint != null)
		{
			Renderer renderer = newWaypoint.GetComponent<Renderer>();
			if (renderer != null)
			{
				MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();


				propertyBlock.SetColor("_BaseColor", teammate.GetTeammateColor());


				renderer.SetPropertyBlock(propertyBlock);
			}

			teammate.AddWaypointMarker(newWaypoint);
		}
		else
		{
			Debug.LogError("Player could not create marker gameObject");
		}


	}


	private void UpdateCameraState(CameraStates state)
	{
		cameraState = state;
	}
}
