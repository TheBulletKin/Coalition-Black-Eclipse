using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControllableEntity : MonoBehaviour
{
	public bool isControlledByPlayer = false;
	public int teammateID;
	
	GameObject characterModel;
	public Transform cameraPos;

	IToggleable[] toggleableComponents;
	[SerializeField] private NavMeshAgent agent;

	private void Start()
	{
		toggleableComponents = GetComponents<IToggleable>();
	}

	public void TakeControl()
	{
		foreach (IToggleable item in toggleableComponents)
		{
			item.EnableControl();
		}
		agent.enabled = false;
	}

	public void LoseControl()
	{
		foreach (IToggleable item in toggleableComponents)
		{
			item.DisableControl();
		}
		agent.enabled = true;
	}

}
