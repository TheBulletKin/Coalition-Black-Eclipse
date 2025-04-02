using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitButton : MainMenuButton
{
	protected override void OnClick()
	{
		Application.Quit();
		Debug.Log("Game manually exited");
	}
}
