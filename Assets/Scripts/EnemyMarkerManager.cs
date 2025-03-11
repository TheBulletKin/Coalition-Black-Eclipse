using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyMarkerManager : MonoBehaviour
{
    private Dictionary<EnemyEntity, EnemyMarkerUiElement> enemyToMarker = new Dictionary<EnemyEntity, EnemyMarkerUiElement>();
	[SerializeField] private GameObject enemyMarkerPrefab;
	[SerializeField] private RectTransform markersContainer;
    [SerializeField] private float markerHeightOffset = 2f;
	public EnemyManager enemyManager;

	private void Start()
	{
		foreach (EnemyEntity entity in enemyManager.enemies)
		{
			//used to subscribe to enemy kill event
			Health entityHealth = entity.GetComponent<Health>();
			entityHealth.OnEnemyDeath += RemoveEntity;

			GameObject newMarker = Instantiate(enemyMarkerPrefab, markersContainer.transform.position, markersContainer.transform.rotation, markersContainer);
			EnemyMarkerUiElement enemyMarkerUiElement = newMarker.GetComponent<EnemyMarkerUiElement>();
			enemyMarkerUiElement.MarkerDisplay.SetActive(entity.isRevealed); //Change this later to be less open
			enemyToMarker.Add(entity, enemyMarkerUiElement);
		}

	}

	private void Update()
	{
		foreach (KeyValuePair<EnemyEntity, EnemyMarkerUiElement> pair in enemyToMarker)
		{
			//For every ai unit in world space, convert to screen space and position the UI element on the player's Ui
			Vector3 screenPosition = Camera.main.WorldToScreenPoint(pair.Key.transform.position + Vector3.up * markerHeightOffset);
			screenPosition.x = Mathf.Clamp(screenPosition.x, 0, Screen.width);
			screenPosition.y = Mathf.Clamp(screenPosition.y, 0, Screen.height);

			//If beyond screen bounds, disable
			if (screenPosition.x >= Screen.width || screenPosition.y >= Screen.width || screenPosition.x <= 0 || screenPosition.y <= 0 || screenPosition.z < 0)
			{
				pair.Value.MarkerDisplay.SetActive(false);
			}
			else
			{
				pair.Value.MarkerDisplay.SetActive(pair.Key.isRevealed);
				pair.Value.transform.position = screenPosition;
			}

		}
	}

	private void RemoveEntity(Health entityToRemove)
	{
		//Didn't want to have to getComponent here
		EnemyEntity enemyToRemove = entityToRemove.GetComponent<EnemyEntity>();
		entityToRemove.OnEnemyDeath -= RemoveEntity;		
		Destroy(enemyToMarker[enemyToRemove].gameObject);
		enemyToMarker.Remove(enemyToRemove);
		
	}
}
