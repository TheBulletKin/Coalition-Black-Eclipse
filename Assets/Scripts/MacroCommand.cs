using System;
using System.Collections.Generic;

/// <summary>
/// A macro command used initially to execute multiple commands in a linear sequence
/// </summary>
public class MacroCommand : ICommand
{
	private List<ICommand> commands = new List<ICommand>();
	private int commandsExecuted = 0;
	private bool isExecuting = false;
	private bool canExecuteNext = true;

	public event Action OnMacroCompleted;
	public event Action<ICommand> OnCommandCompleted;

	/// <summary>
	/// Add the command to the List of commands
	/// </summary>
	/// <param name="command"></param>
	public void AddCommand(ICommand command)
	{
		commands.Add(command);
	}

	/// <summary>
	/// Start executing the first command
	/// </summary>
	public void Execute()
	{
		if (isExecuting)
		{
			return;
		}

		isExecuting = true;
		commandsExecuted = 0;

		ExecuteNextCommand();
	}

	/// <summary>
	/// Executed when the last move command has completed
	/// </summary>
	private void CommandCompleted(ICommand command)
	{
		canExecuteNext = true;

		if (commands.Count > 0)
		{
			if (commands[commandsExecuted - 1] is MoveCommand moveCommand)
			{
				//Unsubscribe to avoid 'ghost calls'
				moveCommand.OnCommandCompleted -= CommandCompleted;
			}

			//Will remove each time currently, but I may choose to keep them there so this is only temporary
			commands.RemoveAt(commandsExecuted - 1);
			commandsExecuted -= 1;
		}

		ExecuteNextCommand();
	}

	/// <summary>
	/// Executed at the start of the execute sequence and after a command has been executed in the sequence
	/// </summary>
	private void ExecuteNextCommand()
	{
		//Only execute if the previous command is done
		if (commandsExecuted < commands.Count)
		{
			if (canExecuteNext == true)
			{
				ICommand currentCommand = commands[commandsExecuted];


				if (currentCommand is MoveCommand moveCommand)
				{
					moveCommand.OnCommandCompleted += CommandCompleted;
				}


				canExecuteNext = false;
				currentCommand.Execute();
				commandsExecuted++;
			}
		}
		else
		{
			//When all of the commands have been executed
			OnMacroCompleted?.Invoke();
			isExecuting = false;
		}
	}

	public void ClearCommands()
	{
		commands.Clear();
	}
}
