using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Android;

public class ObjectiveTracker : MonoBehaviour
{
   public static ObjectiveTracker Instance { get; private set; }

	[SerializeField] private List<Objective> objectives;
	[SerializeField] private bool objectiveComplete;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;

		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		Objective[] objectivesFound = FindObjectsByType<Objective>(FindObjectsSortMode.None);
		foreach (Objective objective in objectivesFound)
		{
			objectives.Add(objective);
		}
	}

	public void CompleteObjective(Objective objective)
	{
		objectives.Remove(objective);
	}

	private void CheckObjectivesComplete()
	{
		if (objectives.Count <= 0)
		{
			objectiveComplete = true;
			GameManager.Instance.FinishGame(EndReason.OBJECTIVE_COMPLETE);
		}
	}

	public void CompleteExtraction()
	{

	}

}
