using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ObjectiveUiManager : MonoBehaviour
{
    public Dictionary<GameObject, ObjectiveMarkerUiElement> objToMarker = new Dictionary<GameObject, ObjectiveMarkerUiElement>();
	public ObjectiveTracker objTracker;

	[SerializeField] private GameObject objMarkerPrefab;
	[SerializeField] private RectTransform objMarkersContainer;
	[SerializeField] private float markerHeightOffset = 2f;
	private void Start()
	{
		objTracker = FindObjectOfType<ObjectiveTracker>();

		foreach (GameObject obj in objTracker.objectives)
		{

			addObjectiveMarker(obj);
		}
	}

	private void Update()
	{
		foreach (KeyValuePair<GameObject, ObjectiveMarkerUiElement> pair in objToMarker)
		{
			//For every ai unit in world space, convert to screen space and position the UI element on the player's Ui
			Vector3 screenPosition = Camera.main.WorldToScreenPoint(pair.Key.transform.position + Vector3.up * markerHeightOffset);
			screenPosition.x = Mathf.Clamp(screenPosition.x, 0, Screen.width);
			screenPosition.y = Mathf.Clamp(screenPosition.y, 0, Screen.height);

			//If beyond screen bounds, disable
			if (screenPosition.x >= Screen.width || screenPosition.y >= Screen.width || screenPosition.x <= 0 || screenPosition.y <= 0 || screenPosition.z < 0)
			{
				pair.Value.marker.SetActive(false);
			}
			else
			{
				pair.Value.marker.SetActive(true);
				pair.Value.transform.position = screenPosition;
			}

		}
	}

	public void addObjectiveMarker(GameObject target)
	{
		GameObject newMarker = Instantiate(objMarkerPrefab, objMarkersContainer.transform.position, objMarkersContainer.transform.rotation, objMarkersContainer);
		ObjectiveMarkerUiElement enemyMarkerUiElement = newMarker.GetComponent<ObjectiveMarkerUiElement>();
		enemyMarkerUiElement.marker.SetActive(true);
		objToMarker.Add(target, enemyMarkerUiElement);
	}

	public void RemoveObjMarker(GameObject obj)
	{
		Destroy(objToMarker[obj].gameObject);
		objToMarker.Remove(obj);
	}
}
