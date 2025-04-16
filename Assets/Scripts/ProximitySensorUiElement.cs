using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProximitySensorUiElement : UiElement, IGadgetUiElement
{    
    public Image sensorIcon;
	public ProximitySensorObject sensorObject;

	public void OnGadgetActivated(IGadget gadget)
	{
		sensorIcon.color = Color.red;
	}

	public void OnGadgetDeactivated(IGadget gadget)
	{
		sensorIcon.color = Color.white;
	}

	public void OnGadgetDestroyed(IGadget gadget)
	{
		DestroyUiElement();
	}

	public void OnGadgetPlaced(IGadget gadget)
	{
		EnableUiElement();
		sensorIcon.color = Color.white;
	}
}
