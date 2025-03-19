using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Android;

public class ObjectiveTracker : MonoBehaviour
{
	public static ObjectiveTracker Instance { get; private set; }

	[SerializeField] public List<GameObject> objectives;
	[SerializeField] private bool objectiveComplete;
	[SerializeField] private ObjectiveUiManager uiManager;
	[SerializeField] private GameObject extractionPoint;

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

	}

	public void CompleteObjective(GameObject objective)
	{
		uiManager.RemoveObjMarker(objective);
		objectives.Remove(objective);
		if (objectives.Count <= 0 && objectiveComplete == false)
		{
			objectiveComplete = true;
			uiManager.addObjectiveMarker(extractionPoint);
		}

	}

	public bool CompleteExtraction()
	{
		if (objectiveComplete == true)
		{
			uiManager.RemoveObjMarker(extractionPoint);
			GameManager.Instance.FinishGame(EndReason.OBJECTIVE_COMPLETE);
			return true;
		}
		else
		{
			return false;
		}
	}

}
