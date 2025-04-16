using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeleportAnchorUiElement : UiElement, IGadgetUiElement
{	
	public TeleportAnchorObject teleportAnchorObject;

	public void OnGadgetActivated(IGadget gadget)
	{
		
	}

	public void OnGadgetDeactivated(IGadget gadget)
	{
		
	}

	public void OnGadgetDestroyed(IGadget gadget)
	{
		DisableUiElement();
	}

	public void OnGadgetPlaced(IGadget gadget)
	{
		EnableUiElement();
	}

	
}
