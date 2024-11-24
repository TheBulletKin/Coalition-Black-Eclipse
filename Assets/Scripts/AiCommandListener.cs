using System.Collections.Generic;
using UnityEngine;

public class AiCommandListener : MonoBehaviour
{
	private List<ICommand> commands = new List<ICommand>();
	bool canRunNextCommand = true;
	bool isSingleCommand;
	bool isGroupCommand;
	int teammatesFinishedCommands = 0;
	[SerializeField] private List<AiCommandListener> aiGroup = new List<AiCommandListener>();

	public List<ICommand> GetCommands()
	{
		return commands;
	}

	public void AddToGroup(AiCommandListener teammateToAdd)
	{
		aiGroup.Add(teammateToAdd);
	}

	public void RunSyncedCommand()
	{
		teammatesFinishedCommands = 0;
		isGroupCommand = true;
		foreach (AiCommandListener teamMember in aiGroup)
		{
			teamMember.RunCommand(true);
		}
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
	public void RunCommand(bool isSingleCommand)
	{
		this.isSingleCommand = isSingleCommand;
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
		
		if (isGroupCommand)
		{
			teammatesFinishedCommands += 1;
			//If every teammate has finished, this will equal the ai group count + this entity
			if (teammatesFinishedCommands == (aiGroup.Count + 1))
			{
				Debug.Log("All group members finished. Executing next from " + gameObject.name);
				teammatesFinishedCommands = 0;
				RunCommand(true);
			}
		} else if (isSingleCommand == false)
		{
			Debug.Log("Executing next task in sequence");
			RunCommand(false);
		}
		

	}


}
