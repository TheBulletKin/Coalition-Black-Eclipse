using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePanelButton : MainMenuButton
{
	[SerializeField] private GameObject uiToClose;
	protected override void OnClick()
	{
		uiToClose.SetActive(false);
	}
}
