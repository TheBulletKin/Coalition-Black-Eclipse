using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipleChoiceItem : MonoBehaviour
{
    public Button showMoreButton;
    public GameObject pairedMenu;

    public void ToggleVisiblity(bool visiblity)
    {
        pairedMenu.SetActive(visiblity);
    }
}
