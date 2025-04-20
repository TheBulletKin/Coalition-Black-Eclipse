using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiCommandListener : MonoBehaviour, IToggleable
{
	private List<ICommand> commands = new List<ICommand>();
	bool canRunNextCommand = true;
	public int groupIndex;
	private int commandsTotal;
	private ICommand currentExecutingCommand;
	public Color teammateColour;
	public string teammateName;
	[SerializeField] private LineRenderer moveWaypointLine;
	[SerializeField] private List<LineRenderer> lookWaypointLines;
	[SerializeField] private float pathUpdateSpeed = 0.25f;
	private float pathUpdateTimer = 0.0f;
	private NavMeshTriangulation triangulation;
	[SerializeField] private GameObject lineRendererHolder;
	[SerializeField] private GameObject lineRendererPrefab;

	private Dictionary<ICommand, GameObject> commandToWaypoint = new Dictionary<ICommand, GameObject>();
	private Dictionary<ICommand, LineRenderer> commandToLineRenderer = new Dictionary<ICommand, LineRenderer>();
	
	private void Awake()
	{
		triangulation = NavMesh.CalculateTriangulation();
	}

	private void Start()
	{
		teammateColour = new Color(teammateColour.r, teammateColour.g, teammateColour.b, 1.0f);
		moveWaypointLine.startColor = teammateColour;
		moveWaypointLine.endColor = teammateColour;
	}
	private void Update()
	{
		commandsTotal = commands.Count;

		if (pathUpdateTimer >= pathUpdateSpeed)
		{
			DrawWaypointPaths();
			pathUpdateTimer = 0.0f;
		} else
		{
			pathUpdateTimer += Time.deltaTime;
		}
	}

	public List<ICommand> GetCommands()
	{
		return commands;
	}

	public void AddCommand(ICommand command)
	{
		commands.Add(command);
	}

	public void ClearCommands()
	{
		commands.Clear();
	}

	/// <summary>
	/// Will get the first command and execute it. Sets up a callback method to execute the same again when it finishes.
	/// Will iterate through all tasks this way.
	/// To control whether only one or a certain number activates, will need to change the CommandComplete method or the RunCommand method to suit my needs.
	/// </summary>
	public void RunCommand()
	{
		if (commands.Count > 0 && canRunNextCommand)
		{
			ICommand command = commands[0];

			command.OnCommandCompleted += CommandCompleted;
			Debug.Log("Starting command on " + gameObject.name);
			canRunNextCommand = false;
			currentExecutingCommand = command;
			command.Execute(this);
		}
	}

	public void RunCommand(ICommand command)
	{
		if (currentExecutingCommand != null)
		{
			currentExecutingCommand.Cancel(this);
		}

		command.OnCommandCompleted += CommandCompleted;
		Debug.Log("Starting command on " + gameObject.name);

		currentExecutingCommand = command;
		command.Execute(this);
	}

	/// <summary>
	/// Callback method that runs when the command finishes execution, called by the command itself
	/// </summary>
	/// <param name="command"></param>
	private void CommandCompleted(ICommand command)
	{
		canRunNextCommand = true;
		command.OnCommandCompleted -= CommandCompleted;

		if (commandToWaypoint.ContainsKey(command))
		{
			Destroy(commandToWaypoint[command]);
			commandToWaypoint.Remove(command);
		}

		if (commandToLineRenderer.ContainsKey(command))
		{
			Destroy(commandToLineRenderer[command].gameObject);
			commandToLineRenderer.Remove(command);
		}


		currentExecutingCommand = null;
		commands.Remove(command);
		Debug.Log("Command completed. Executing next task in sequence in " + gameObject.name);

		DrawWaypointPaths();

		RunCommand();
	}

	public void AddWaypointMarker(ICommand command, GameObject marker)
	{
		//Look commands require a line renderer separate to the others, so I create a new one here
		if (command is LookCommand && !commandToLineRenderer.ContainsKey(command))
		{
			Quaternion newRotation = Quaternion.Euler(new Vector3(gameObject.transform.rotation.x + 90f, gameObject.transform.rotation.y, gameObject.transform.rotation.z));
			GameObject newLineRendererObject = Instantiate(lineRendererPrefab, gameObject.transform.position, newRotation);

			LineRenderer newLineRenderer = newLineRendererObject.AddComponent<LineRenderer>();

			newLineRenderer.startWidth = moveWaypointLine.startWidth;
			newLineRenderer.endWidth = moveWaypointLine.endWidth;

			newLineRenderer.material = moveWaypointLine.material;

			//Setting it manually, will need some kind of config to make this easier and editable in the inspector
			Color newWaypointColour = new Color(teammateColour.r, teammateColour.g, teammateColour.b, 0.5f);
			newLineRenderer.startColor = newWaypointColour;
			newLineRenderer.endColor = newWaypointColour;

			newLineRenderer.alignment = moveWaypointLine.alignment;

			commandToLineRenderer.Add(command, newLineRenderer);
		}
		commandToWaypoint.Add(command, marker);

		DrawWaypointPaths();
	}

	public Color GetTeammateColor()
	{
		return teammateColour;
	}

	/// <summary>
	/// Clear and redraw all waypoint paths from start to finish
	/// </summary>
	private void DrawWaypointPaths()
	{
		NavMeshPath path = new NavMeshPath();

		Vector3 startPosition = Vector3.zero;

		//For now it will recalculate the whole path. Make it more efficient later		
		moveWaypointLine.positionCount = 0;
		int currentPositionIndex = 0;

		bool firstLineDraw = true;

		foreach (ICommand command in commands)
		{
			if (firstLineDraw)
			{
				startPosition = gameObject.transform.position;
			}

			if (command is MoveCommand)
			{
				DrawPaths(startPosition, ref path, commandToWaypoint[command].transform.position, ref currentPositionIndex);

				startPosition = commandToWaypoint[command].transform.position;
				firstLineDraw = false;

			}
			else if (command is LookCommand)
			{
				LineRenderer lookLineRenderer = commandToLineRenderer[command];
				lookLineRenderer.SetPosition(0, startPosition + Vector3.up * 0.2f);
				lookLineRenderer.SetPosition(1, commandToWaypoint[command].transform.position + Vector3.up * 0.2f);

			}
		}
	}

	private void DrawPaths(Vector3 startPosition, ref NavMeshPath path, Vector3 targetPosition, ref int currentPositionIndex)
	{
		if (NavMesh.CalculatePath(startPosition, targetPosition, NavMesh.AllAreas, path))
		{
			int pathLength = path.corners.Length;
			moveWaypointLine.positionCount += pathLength;

			for (int i = 0; i < path.corners.Length; i++)
			{
				moveWaypointLine.SetPosition(currentPositionIndex++, path.corners[i] + Vector3.up * 0.2f);
			}
		}
		else
		{
			Debug.LogError(gameObject.name + " was unable to calculate path for line renderer");
		}
	}

	public void DisableControl()
	{
		enabled = true;
	}

	public void EnableControl()
	{
		enabled = false;
	}

	public void CancelCommands()
	{
		foreach (KeyValuePair<ICommand, GameObject> commandWaypoint in commandToWaypoint)
		{
			Destroy(commandWaypoint.Value);
		}

		commandToWaypoint.Clear();

		foreach (KeyValuePair<ICommand, LineRenderer> commandLineRenderer in commandToLineRenderer)
		{
			Destroy(commandLineRenderer.Value.gameObject);
		}

		commandToLineRenderer.Clear();

		if (currentExecutingCommand != null)
		{
			currentExecutingCommand.Cancel(this);
		}

		canRunNextCommand = true;
		commands.Clear();
	}
}
