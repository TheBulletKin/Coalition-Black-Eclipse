using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Android;

public class ObjectiveTracker : MonoBehaviour
{
   public static ObjectiveTracker Instance { get; private set; }

	[SerializeField] private Objective objective;
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

	public void CompleteObjective(Objective objective)
	{
		objectiveComplete = true;
		GameManager.Instance.FinishGame(EndReason.OBJECTIVE_COMPLETE);
	}

	public void CompleteExtraction()
	{

	}

}
