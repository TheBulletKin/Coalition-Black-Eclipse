using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public List<MainMenuButton> buttons;

    // Start is called before the first frame update
    void Start()
    {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = false;
	}

    
}
