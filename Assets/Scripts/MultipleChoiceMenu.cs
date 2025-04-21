using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipleChoiceMenu : MonoBehaviour
{
    public List<MultipleChoiceItem> listItems;

    //Added temporarily so the the cursor remains unlocked on the main menu
	private void Start()
	{
		Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
	}

	public void ShowSelectedDetails(MultipleChoiceItem listItem)
    {
        foreach (MultipleChoiceItem item in listItems) 
        {
            if (item == listItem) 
            {
                item.ToggleVisiblity(true);
            }
            else
            {
                item.ToggleVisiblity(false);
            }

        }

    }
}
