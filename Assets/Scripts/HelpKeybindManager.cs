using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpKeybindManager : MonoBehaviour
{
	//Temporary just for demo sake
	public GameObject keybindPanel;
	public GameObject helpPanelKeybind;
	public TextMeshProUGUI keybindText;


	//temp to act as menu manager for escape key press
	public GameObject exitPanel;
	private bool exitMenuOpened;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.H))
		{
			keybindPanel.SetActive(!keybindPanel.activeSelf);

			//helpPanelKeybind.SetActive(!helpPanelKeybind.activeSelf);
			if (keybindPanel.activeSelf)
			{
				keybindText.text = "Hide Keybinds";
			}
			else
			{
				keybindText.text = "Show Keybinds";
			}


		}

		if (exitMenuOpened)
		{
			if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.Escape))
			{
				SceneManager.LoadScene("MainMenu");
			}

			if (Input.GetKeyDown(KeyCode.N))
			{
				exitMenuOpened = false;
				exitPanel.SetActive(false);
			}
		}

		if (!exitMenuOpened)
		{

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				exitPanel.SetActive(true);
				exitMenuOpened = true;
			}

		}

	}
}
