using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractionPoint : MonoBehaviour, IInteractable
{
	public void Interact(GameObject instigator)
	{
		if (ObjectiveTracker.Instance.CompleteExtraction())
		{
			Destroy(gameObject);
		}

	}
}
