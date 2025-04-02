using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Main menu button that other button types will extend.
 * Base class will assign the OnClick method to the onClick event for the button itself.
 */
public abstract class MainMenuButton : MonoBehaviour
{
    protected Button button;

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    //Assigns this method, but using polymorphism it will call the overriden method instead.
    protected abstract void OnClick();
}
