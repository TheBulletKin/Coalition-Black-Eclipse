using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiTeammateCard : UiElement
{
    public TextMeshProUGUI teamIndexText;
	public Color teammateColour { set; private get; }
    public Image background;
    public TextMeshProUGUI nameText;

    public void ToggleAsActiveTeammate(bool state)
    {
        if (state == true)
        {
            background.color = Color.yellow;
        }
        else
        {
            background.color = teammateColour;
        }
    }

    

   
}
