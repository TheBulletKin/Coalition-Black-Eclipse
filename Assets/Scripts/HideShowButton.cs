using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideShowButton : MainMenuButton
{
	bool uiVisible = false;
	[Tooltip("If the button should invert the visibility of the ui element")]
	[SerializeField] private bool shouldToggle = false;
	[Tooltip("If the button should reveal or hide the ui element to change")]
	[SerializeField] private bool clickReveals = true;
	[Tooltip("If the button should close the ui group it is in currently")]
	[SerializeField] private bool closesCurrent = false;
	[SerializeField] private GameObject uiToClose;
	[Tooltip("The ui element to change")]
	[SerializeField] private GameObject uiToToggle;
	
	protected override void OnClick()
	{
		if (shouldToggle)
		{
			uiVisible = !uiVisible;
			uiToToggle.SetActive(true);
		}
		else
		{
			uiToToggle.SetActive(clickReveals);
			if (closesCurrent)
			{
				uiToClose.SetActive(false);
			}

		}
	}

	private void Start()
	{
		
	}


}
