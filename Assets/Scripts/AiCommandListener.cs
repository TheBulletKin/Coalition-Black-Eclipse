using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiCommandListener : MonoBehaviour
{
	private List<ICommand> commands = new List<ICommand>();
	bool canRunNextCommand = true;
	public int groupIndex;
	public int commandsTotal;
	public ICommand currentExecutingCommand;
	public Color waypointColour;
	public LineRenderer waypointPath;
	[SerializeField]
	private float pathUpdateSpeed = 0.25f;
	private float pathUpdateTimer = 0.0f;
	private NavMeshTriangulation triangulation;

	private Dictionary<ICommand, GameObject> commandToWaypoint = new Dictionary<ICommand, GameObject>();

	private void Awake()
	{
		triangulation = NavMesh.CalculateTriangulation();

	}

	private void Start()
	{
		Color waypointColourWithAlpha = new Color(waypointColour.r, waypointColour.g, waypointColour.b, 1.0f);
		waypointPath.startColor = waypointColourWithAlpha;
		waypointPath.endColor = waypointColourWithAlpha;

	}
	private void Update()
	{
		commandsTotal = commands.Count;
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
			command.Execute(this);
			canRunNextCommand = false;
			currentExecutingCommand = command;
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
		command.Execute(this);
		currentExecutingCommand = command;

	}


	private void CommandCompleted(ICommand command)
	{
		canRunNextCommand = true;
		command.OnCommandCompleted -= CommandCompleted;

		if (commandToWaypoint.ContainsKey(command))
		{
			Destroy(commandToWaypoint[command]);
			commandToWaypoint.Remove(command);
		}


		currentExecutingCommand = null;
		commands.Remove(command);
		Debug.Log("Command completed. Executing next task in sequence in " + gameObject.name);

		DrawWaypointPaths();

		RunCommand();


	}

	public void AddWaypointMarker(GameObject marker)
	{
		foreach (ICommand command in commands)
		{
			if (!commandToWaypoint.ContainsKey(command))
			{
				commandToWaypoint.Add(command, marker);
				DrawWaypointPaths();
				break;
			}
		}
	}

	public Color GetTeammateColor()
	{
		return waypointColour;
	}

	private void DrawWaypointPaths()
	{
		NavMeshPath path = new NavMeshPath();

		Vector3 startPosition = gameObject.transform.position;

		//For now it will recalculate the whole path. Make it more efficient later		
		waypointPath.positionCount = 0;
		int currentPositionIndex = 0;

		bool drawFromPlayer = true;

		foreach (ICommand command in commands)
		{

			if (drawFromPlayer)
			{


				DrawPaths(startPosition, ref path, commandToWaypoint[command].transform.position, ref currentPositionIndex);

				startPosition = commandToWaypoint[command].transform.position;
				drawFromPlayer = false;


			}
			else
			{
				DrawPaths(startPosition, ref path, commandToWaypoint[command].transform.position, ref currentPositionIndex);
				startPosition = commandToWaypoint[command].transform.position;
			}





		}


	}

	private void DrawPaths(Vector3 startPosition, ref NavMeshPath path, Vector3 targetPosition, ref int currentPositionIndex)
	{
		if (NavMesh.CalculatePath(startPosition, targetPosition, NavMesh.AllAreas, path))
		{
			int pathLength = path.corners.Length;
			waypointPath.positionCount += pathLength;

			for (int i = 0; i < path.corners.Length; i++)
			{
				waypointPath.SetPosition(currentPositionIndex++, path.corners[i] + Vector3.up * 0.2f);
			}

		}
		else
		{
			Debug.LogError(gameObject.name + " was unable to calculate path for line renderer");
		}
	}


}
