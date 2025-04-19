using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipleChoiceMenu : MonoBehaviour
{
    public List<MultipleChoiceItem> listItems;

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
