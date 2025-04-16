using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGadgetUiElement
{
	void OnGadgetPlaced(IGadget gadget);
	void OnGadgetActivated(IGadget gadget);
	void OnGadgetDeactivated(IGadget gadget);
	void OnGadgetDestroyed(IGadget gadget);

	
}
