using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyMarkerManager : MonoBehaviour, IInitialisable
{
	[SerializeField] private GameObject enemyMarkerPrefab;
	[SerializeField] private RectTransform markersContainer;
	[SerializeField] private float markerHeightOffset = 2f;
	public EnemyManager enemyManager;

	private List<EnemyMarkerUiElement> markers = new List<EnemyMarkerUiElement>();

	//Even though the marker element holds a reference, a dictionary saves performance when deleting the ui element
	private Dictionary<EnemyEntity, EnemyMarkerUiElement> enemyToMarker = new Dictionary<EnemyEntity, EnemyMarkerUiElement>();


	/// <summary>
	/// Requires: EnemyManager
	/// </summary>
	/// <returns></returns>
	public void Initialize()
	{

		foreach (EnemyEntity entity in enemyManager.enemies)
		{
			//used to subscribe to enemy kill event
			Health entityHealth = entity.GetComponent<Health>();
			entityHealth.OnEntityDeath += RemoveEntity;

			GameObject newMarker = Instantiate(enemyMarkerPrefab, markersContainer.transform.position, markersContainer.transform.rotation, markersContainer);
			EnemyMarkerUiElement enemyMarkerUiElement = newMarker.GetComponent<EnemyMarkerUiElement>();
			enemyMarkerUiElement.attachedEnemy = entity;
			enemyMarkerUiElement.SetWorldSpaceTracking(true, entity.transform, false);
			//lambda expressions because callback setters can't have arguments
			enemyMarkerUiElement.SetScreenExitCallback(HandleScreenExit);
			enemyMarkerUiElement.SetScreenEnterCallback(HandleScreenEnter);
			enemyMarkerUiElement.SetVisibilityState(false);

			entity.OnEnemyRevealed += ActivateMarkerForEnemy;

			markers.Add(enemyMarkerUiElement);
			enemyToMarker.Add(entity, enemyMarkerUiElement);
		}

	}

	

	private void ActivateMarkerForEnemy(EnemyEntity entity)
	{
		enemyToMarker[entity].SetVisibilityState(true);
	}

	private void HandleScreenEnter(UiElement element)
	{
		if (element is EnemyMarkerUiElement marker)
		{
			marker.SetVisibilityState(marker.attachedEnemy.isRevealed);
		}
	}

	private void HandleScreenExit(UiElement element, bool tracksWhileOffScreen)
	{

		element.SetVisibilityState(false);


	}

	private void Update()
	{

	}

	private void RemoveEntity(Health entityHealth)
	{
		entityHealth.OnEntityDeath -= RemoveEntity;

		EnemyEntity enemyEntity = entityHealth.GetComponent<EnemyEntity>();
		enemyEntity.OnEnemyRevealed -= ActivateMarkerForEnemy;

		EnemyMarkerUiElement markerToRemove = enemyToMarker[enemyEntity];
		markers.Remove(markerToRemove);
		enemyToMarker.Remove(enemyEntity);

		markerToRemove.DestroyUiElement();
	}

	private void OnDestroy()
	{
		markers.Clear();
		enemyToMarker.Clear();
	}
}
