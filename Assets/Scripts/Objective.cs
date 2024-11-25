using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour, IInteractable
{
    
	public void Interact()
    {
        ObjectiveTracker.Instance.CompleteObjective(this);
        Destroy(gameObject);
    }

    
}
