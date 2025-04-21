using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class responsible for assigning commands to Ai teammates and telling them when to begin executing tasks and how.
/// </summary>
public class PlayerCommandIssuer : MonoBehaviour
{
	[SerializeField] private List<AiCommandListener> aiTeammates;
	public float interactionRange = 3f;
	public LayerMask interactableLayer;
	public int currentGroupOrTeammateIndex;

	//Temporary for ability command
	public int currentAbilityIndex;
	public bool selectingGroup;
	public GameObject moveMarker;
	public GameObject lookMarker;
	public GameObject abilityMarker;

	public Action<int, bool> OnTeammateOrGroupChanged;



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

		InputManager.Instance.OnCommandCancelPressed += CancelAllCommands;


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

		InputManager.Instance.OnCommandCancelPressed -= CancelAllCommands;
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
		OnTeammateOrGroupChanged?.Invoke(groupIndex, true);
		Debug.Log("Group selected: " + groupIndex);
	}

	private void ChangeCurrentTeammate(int teammateIndex)
	{
		currentGroupOrTeammateIndex = teammateIndex;
		selectingGroup = false;
		OnTeammateOrGroupChanged?.Invoke(teammateIndex, false);
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
						ICommand newCommand = CreateCommandFromType(teammate, commandType);
						teammate.AddCommand(newCommand);
						CreateCommandWaypoint(newCommand, teammate, commandType);
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
				ICommand newCommand = CreateCommandFromType(aiTeammates[currentGroupOrTeammateIndex], commandType);
				
				aiTeammates[currentGroupOrTeammateIndex].AddCommand(newCommand);				

				CreateCommandWaypoint(newCommand, aiTeammates[currentGroupOrTeammateIndex], commandType);
			}
			else
			{
				aiTeammates[currentGroupOrTeammateIndex].RunCommand(CreateCommandFromType(aiTeammates[currentGroupOrTeammateIndex], commandType));
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="entity">The ai the command is being assigned to</param>
	/// <param name="commandType">Defines what command class to create</param>
	/// <returns>Command class relating to commandType</returns>
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
			case CommandType.ABILITY:
				targetPosition = TryGetSelectedPosition();
				AbilitySystem abilitySystem = entity.GetComponent<AbilitySystem>();
				newCommand = new AbilityCommand(abilitySystem, currentAbilityIndex, targetPosition); //How can I better pass the ability to this?
				return newCommand;
			case CommandType.NONE:
				return null;
			default:
				return null;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="command">Command that the waypoint represents</param>
	/// <param name="teammate">The ai the waypoint is being created for</param>
	/// <param name="commandType">Determines which waypoint to instantiate</param>
	private void CreateCommandWaypoint(ICommand command, AiCommandListener teammate, CommandType commandType)
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
			case CommandType.ABILITY:
				newWaypoint = Instantiate(abilityMarker, targetPosition, Quaternion.identity);
				break;
			case CommandType.NONE:
				break;
			default:
				break;
		}

		if (newWaypoint != null)
		{
			//Change the colour to match the teammate's colour
			Renderer renderer = newWaypoint.GetComponent<Renderer>();
			if (renderer != null)
			{
				MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

				propertyBlock.SetColor("_BaseColor", teammate.GetTeammateColor());

				renderer.SetPropertyBlock(propertyBlock);
			}

			teammate.AddWaypointMarker(command, newWaypoint);
		}
		else
		{
			Debug.LogError("Player could not create marker gameObject");
		}


	}

	public List<AiCommandListener> GetTeammates()
	{
		return aiTeammates;
	}

	private void UpdateCameraState(CameraStates state)
	{
		cameraState = state;
	}

	public void CancelAllCommands()
	{
		foreach (AiCommandListener teammate in aiTeammates)
		{
			if (aiTeammates[currentGroupOrTeammateIndex] == teammate)
			{
				teammate.CancelCommands();

			}
		}
	}
}
