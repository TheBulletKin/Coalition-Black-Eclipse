using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControllableEntity : MonoBehaviour
{
	public bool isControlledByPlayer = false;
	public int teammateID;
	
	public GameObject characterModel;
	public Transform cameraPos;

	IToggleable[] toggleableComponents;
	[SerializeField] private NavMeshAgent agent;

	private void Start()
	{
		toggleableComponents = GetComponents<IToggleable>();
	}

	public void TakeControl()
	{
		if (toggleableComponents == null)
		{
			toggleableComponents = GetComponents<IToggleable>();
		}
		foreach (IToggleable item in toggleableComponents)
		{
			item.EnableControl();
		}
		agent.enabled = false;
	}

	public void LoseControl()
	{
		if (toggleableComponents == null)
		{
			toggleableComponents = GetComponents<IToggleable>();
		}
		foreach (IToggleable item in toggleableComponents)
		{
			item.DisableControl();
		}
		agent.enabled = true;
	}

}
