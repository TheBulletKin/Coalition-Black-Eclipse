using System.Collections.Generic;
using UnityEngine;

public class AiCommandListener : MonoBehaviour
{
	private List<ICommand> commands = new List<ICommand>();
	bool canRunNextCommand = true;
	public int groupIndex;
	public int commandsTotal;
	public ICommand currentExecutingCommand;
	public Color waypointColour;	

	private Dictionary<ICommand, GameObject> commandToWaypoint = new Dictionary<ICommand, GameObject>();

	private void Start()
	{
		
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
		
		Destroy(commandToWaypoint[command]);
		commandToWaypoint.Remove(command);

		currentExecutingCommand = null;
		commands.Remove(command);
		Debug.Log("Command completed. Executing next task in sequence in " + gameObject.name);
		
		RunCommand();


	}

	public void AddWaypointMarker(GameObject marker)
	{
		foreach (ICommand command in commands)
		{
			if (!commandToWaypoint.ContainsKey(command))
			{
				commandToWaypoint.Add(command, marker);
				break;
			}
		}
	}

	public Color GetTeammateColor()
	{
		return waypointColour;
	}


}
