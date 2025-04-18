using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProximitySensorUiElement : UiElement, IGadgetUiElement
{    
    public Image sensorIcon;
	public ProximitySensorObject sensorObject;
	public Color defaultInnerColour;
	public Color alertInnerColour;

	public void OnGadgetActivated(IGadget gadget)
	{
		sensorIcon.color = alertInnerColour;
	}

	public void OnGadgetDeactivated(IGadget gadget)
	{
		sensorIcon.color = defaultInnerColour;
	}

	public void OnGadgetDestroyed(IGadget gadget)
	{
		DestroyUiElement();
	}

	public void OnGadgetPlaced(IGadget gadget)
	{
		EnableUiElement();
		sensorIcon.color = defaultInnerColour;
	}
}
