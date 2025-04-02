using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsButton : MainMenuButton
{
	bool controlsShown = false;
	[SerializeField] private GameObject controlsScreen;
	protected override void OnClick()
	{
		//controlsShown = !controlsShown;
		controlsScreen.SetActive(true);
	}

	private void Start()
	{
		controlsScreen.SetActive(false);
	}


}
