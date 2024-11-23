using System.Collections.Generic;
using UnityEngine;

public class AiCommandListener : MonoBehaviour
{
	[SerializeField] private List<ICommand> commands = new List<ICommand>();
	bool canRunNextCommand = true;

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

		if (commands.Count > 0)
		{
			ICommand command = commands[0];
			command.OnCommandCompleted += CommandCompleted;
			command.Execute();
		}
		

	}

	

	private void CommandCompleted(ICommand command)
	{
		canRunNextCommand = true;
		command.OnCommandCompleted -= CommandCompleted;
		commands.Remove(command);

		Debug.Log("Command completed");
		RunCommand();
	}
}
