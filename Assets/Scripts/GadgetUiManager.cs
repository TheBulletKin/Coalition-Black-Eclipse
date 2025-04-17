using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GadgetUiManager : MonoBehaviour
{
	[SerializeField] private GameObject sensorMarkerPrefab;
	[SerializeField] private GameObject anchorMarkerPrefab;
	[SerializeField] private GameObject subterfugeUiPrefab;
	[SerializeField] private RectTransform gadgetMarkerContainer;
	[SerializeField] private RectTransform statusEffectContainer;
	[SerializeField] private float offScreenElementRadiusPercentage = 0.75f;
	private float floatingUiElementRadius;
	[SerializeField] private float offScreenElementScale = 0.5f;
	[SerializeField] private float minScale = 0.5f;
	[SerializeField] private float maxScale = 1.0f;
	[SerializeField] private float maxDistance = 50f;
	[SerializeField] private float distanceBeforeDownscaling = 10f;

	private List<UiElement> gadgetUiElements = new List<UiElement>();

	private Dictionary<IGadget, IGadgetUiElement> gadgetToUi = new Dictionary<IGadget, IGadgetUiElement>();


	public IGadgetUiElement CreateMarker(IGadget gadget)
	{
		if (gadget is TeleportAnchorObject anchorObject)
		{
			GameObject teleportAnchorUiObject = Instantiate(anchorMarkerPrefab, gadgetMarkerContainer.transform.position, gadgetMarkerContainer.transform.rotation, gadgetMarkerContainer);

			TeleportAnchorUiElement anchorUiElement = teleportAnchorUiObject.GetComponent<TeleportAnchorUiElement>();
			anchorUiElement.SetVisibilityState(true);
			anchorUiElement.SetWorldSpaceTracking(true, anchorObject.transform, true);
			anchorUiElement.teleportAnchorObject = anchorObject;
			anchorUiElement.SetScreenExitCallback(HandleScreenExit);
			anchorUiElement.SetScreenEnterCallback(HandleScreenEnter);
			anchorUiElement.SetScaling(true, minScale, maxScale, maxDistance, distanceBeforeDownscaling);

			anchorUiElement.offScreenFloatingRadius = floatingUiElementRadius;

			gadgetUiElements.Add(anchorUiElement);
			gadgetToUi.Add(gadget, anchorUiElement);
			return anchorUiElement;
		}
		else if (gadget is ProximitySensorObject sensorObject)
		{
			GameObject proximitySensorUiObject = Instantiate(sensorMarkerPrefab, gadgetMarkerContainer.transform.position, gadgetMarkerContainer.transform.rotation, gadgetMarkerContainer);

			ProximitySensorUiElement sensorUiElement = proximitySensorUiObject.GetComponent<ProximitySensorUiElement>();
			sensorUiElement.SetVisibilityState(true);
			sensorUiElement.SetWorldSpaceTracking(true, sensorObject.transform, true);
			sensorUiElement.sensorObject = sensorObject;
			sensorUiElement.SetScreenExitCallback(HandleScreenExit);
			sensorUiElement.SetScreenEnterCallback(HandleScreenEnter);
			sensorUiElement.SetScaling(true, minScale, maxScale, maxDistance, distanceBeforeDownscaling);

			sensorUiElement.offScreenFloatingRadius = floatingUiElementRadius;

			gadgetUiElements.Add(sensorUiElement);
			gadgetToUi.Add(gadget, sensorUiElement);
			return sensorUiElement;
		}
		else if (gadget is SubterfugeAbility subterfuge)
		{
			if (!gadgetToUi.ContainsKey(subterfuge))
			{
				GameObject subterfugeUiObject = Instantiate(subterfugeUiPrefab, statusEffectContainer.transform.position, statusEffectContainer.transform.rotation, statusEffectContainer);

				SubterfugeUiElement subterfugeUiElement = subterfugeUiObject.GetComponent<SubterfugeUiElement>();

				subterfugeUiElement.SetVisibilityState(true);
				subterfugeUiElement.SetWorldSpaceTracking(false);
				subterfugeUiElement.visibility = subterfuge.affectedEntity;

				gadgetUiElements.Add(subterfugeUiElement);
				gadgetToUi.Add(gadget, subterfugeUiElement);

				return subterfugeUiElement;
			}

			return null;
			
			
		}
		else
		{
			return null;
		}

	}

	private void Start()
	{
		//When off screen, ui elements will form a ring around the screen centre. This sets that radius based on screen size
		floatingUiElementRadius = Mathf.Min(Screen.width, Screen.height) * offScreenElementRadiusPercentage;
	}

	private void Update()
	{
		foreach (UiElement element in gadgetUiElements)
		{
			if (element is ProximitySensorUiElement)
			{

			}
			else if (element is TeleportAnchorUiElement)
			{

			}
			else if (element is SubterfugeUiElement subterfuge)
			{

			}
		}
	}

	private void OnEnable()
	{
		GameEvents.OnGadgetPlaced += HandleGadgetPlaced;
		GameEvents.OnGadgetActivated += HandleGadgetActivated;
		GameEvents.OnGadgetDeactivated += HandleGadgetDeactivated;
		GameEvents.OnGadgetDestroyed += HandleGadgetDestroyed;
	}

	private void OnDisable()
	{
		GameEvents.OnGadgetPlaced -= HandleGadgetPlaced;
		GameEvents.OnGadgetActivated -= HandleGadgetActivated;
		GameEvents.OnGadgetDeactivated -= HandleGadgetDeactivated;
		GameEvents.OnGadgetDestroyed -= HandleGadgetDestroyed;
	}


	//---- Global events
	//All these methods are fired in response to placing and destroying gadgets in other scripts
	private void HandleGadgetPlaced(IGadget gadget)
	{
		CreateMarker(gadget);
		gadgetToUi[gadget].OnGadgetPlaced(gadget);
	}

	private void HandleGadgetActivated(IGadget gadget)
	{
		gadgetToUi[gadget].OnGadgetActivated(gadget);
	}

	private void HandleGadgetDeactivated(IGadget gadget)
	{
		gadgetToUi[gadget].OnGadgetDeactivated(gadget);
	}

	private void HandleGadgetDestroyed(IGadget gadget)
	{
		if (gadgetToUi.TryGetValue(gadget, out IGadgetUiElement uiElement))
		{
			//Type casting from interface to concrete class
			UiElement element = uiElement as UiElement;
			if (element != null)
			{
				Destroy(element.gameObject);
				gadgetUiElements.Remove(uiElement as UiElement);
				gadgetToUi.Remove(gadget);
			}			
		}

	}

	private void HandleScreenEnter(UiElement element)
	{
		/* Left here for if I want specific behaviour based on element type
		if (element is EnemyMarkerUiElement marker)
		{
			marker.SetVisibilityState(marker.attachedEnemy.isRevealed);
		}*/
		element.SetScaling(true, minScale, maxScale, maxDistance, distanceBeforeDownscaling);
		element.SetVisibilityState(true);
		element.uiElement.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	private void HandleScreenExit(UiElement element, bool tracksWhileOffScreen)
	{
		if (tracksWhileOffScreen)
		{
			element.SetScaling(false, 1.0f, 1.0f, 10f, 5);
			element.uiElementRectTransform.localScale = new Vector3(offScreenElementScale, offScreenElementScale, offScreenElementScale);
		}
		else
		{
			element.SetVisibilityState(false);
		}

	}
}
