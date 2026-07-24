using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour, IInitialisable
{
	public static TeamManager Instance { get; private set; }
	public List<ControllableEntity> playerTeammates;
	public List<Observable> groupObserved = new List<Observable>();

	float prevTime;
	public void Initialize()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		ControllableEntity[] foundTeammates = FindObjectsByType<ControllableEntity>(FindObjectsSortMode.InstanceID);
		foreach (ControllableEntity teammate in foundTeammates)
		{
			Instance.playerTeammates.Add(teammate);

		}
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		HandleGroupObservations();
	}

	public void HandleGroupObservations()
	{

		float deltaTime = Time.deltaTime;
		groupObserved.Clear();
		foreach (ControllableEntity entity in playerTeammates)
		{
			foreach (Observable observedEntity in entity.aiDetection.ObservedThings)
			{
				if (!groupObserved.Contains(observedEntity))
				{
					groupObserved.Add(observedEntity);
				}
			}

		}



		foreach (Observable obs in groupObserved)
		{
			if (obs.type == ObservableType.ROOM_MEMORYNODE)
			{
				MemoryNode memoryNode = obs.GetComponent<MemoryNode>();
				memoryNode.clearFactor += deltaTime;
				if (memoryNode.clearFactor >= 2.0f)
				{
					memoryNode.clearFactor = 2.0f;
				}
			}
		}
	}
}
