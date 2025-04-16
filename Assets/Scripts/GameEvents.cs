using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
	public static Action<IGadget> OnGadgetPlaced;
	public static Action<IGadget> OnGadgetDestroyed;
	public static Action<IGadget> OnGadgetActivated;
	public static Action<IGadget> OnGadgetDeactivated;

	public static void TriggerGadgetPlaced(IGadget gadget)
	{
		OnGadgetPlaced?.Invoke(gadget);
	}
}
