using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ICommand
{
	void Execute(MonoBehaviour executor);
	void Cancel(MonoBehaviour executor);
	public event Action<ICommand> OnCommandCompleted;
}
