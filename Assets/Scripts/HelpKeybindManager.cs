using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpKeybindManager : MonoBehaviour
{
	//Temporary just for demo sake
    public GameObject keybindPanel;
	public GameObject helpPanelKeybind;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.H))
		{
			keybindPanel.SetActive(!keybindPanel.activeSelf);
			helpPanelKeybind.SetActive(!helpPanelKeybind.activeSelf);
		}
	}
}
