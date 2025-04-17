using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiElement : MonoBehaviour
{
	[SerializeField] private bool trackWorldPosition = false;
	public Vector3 worldPositionOffset;
	public Transform worldTarget;
	public bool isBlockedByGeometry = false;
	public LayerMask occluderLayers;
	public bool scalesWithDistance = false;
	public float minScale = 0.2f;
	public float maxScale = 1.0f;
	public float maxDistance = 50f;
	[SerializeField] private float distanceBeforeDownscaling = 10f;
	[SerializeField] private bool shouldDisplay = false;
	[SerializeField] public GameObject uiElement;
	[SerializeField] public RectTransform uiElementRectTransform;
	private bool wasOnScreenLastFrame = false;
	private bool isOnScreen = false;
	private bool isBehindPlayer = false;
	public bool tracksWhileOffScreen = false;
	public float offScreenFloatingRadius = 0.25f;
	

	public Action<UiElement> onUiElementDestroyed;
	public Action<UiElement, bool> onScreenExit;
	public Action<UiElement> onScreenEnter;

	private void Start()
	{
		uiElementRectTransform = uiElement.GetComponent<RectTransform>();
	}


	private void Update()
	{
		if (trackWorldPosition && worldTarget != null)
		{
			Vector3 worldPos = worldTarget.position + worldPositionOffset;
			Vector3 screenPos = Vector3.zero;
			if (Camera.main)
			{
				screenPos = Camera.main.WorldToScreenPoint(worldPos);
			}
			
			Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

			if (scalesWithDistance && uiElementRectTransform != null)
			{
				float distance = Vector3.Distance(worldTarget.position, Camera.main.transform.position);

				//keep max scale until this point
				if (distance <= distanceBeforeDownscaling)
				{
					uiElementRectTransform.localScale = Vector3.one * maxScale;
				}
				else
				{					
					float t = Mathf.Clamp01((distance - distanceBeforeDownscaling) / (maxDistance - distanceBeforeDownscaling));
					float scale = Mathf.Lerp(maxScale, minScale, t);
					uiElementRectTransform.localScale = Vector3.one * scale;
				}
			}


			isBehindPlayer = screenPos.z < 0;

			// If behind player, flip the direction vector instead of flipping the world position
			Vector3 dirFromCenter = (screenPos - screenCenter).normalized;
			if (isBehindPlayer)
			{
				dirFromCenter *= -1f;
				screenPos = screenCenter + dirFromCenter * (offScreenFloatingRadius);
				screenPos.z = 0f;
			}

			isOnScreen = screenPos.z > 0 &&
						 screenPos.x >= 0 && screenPos.x <= Screen.width &&
						 screenPos.y >= 0 && screenPos.y <= Screen.height;

			if (!isOnScreen)
			{
				// Keep the direction to edge
				Vector3 clampedPos = screenCenter + dirFromCenter * (offScreenFloatingRadius - 10f);
				screenPos = new Vector3(
					Mathf.Clamp(clampedPos.x, 0, Screen.width),
					Mathf.Clamp(clampedPos.y, 0, Screen.height),
					0f
				);
			}

			if (!isOnScreen && wasOnScreenLastFrame)
			{
				onScreenExit?.Invoke(this, tracksWhileOffScreen);
			}
			else if (isOnScreen && !wasOnScreenLastFrame)
			{
				onScreenEnter?.Invoke(this);
			}

			if (isOnScreen || tracksWhileOffScreen || isBehindPlayer)
			{
				transform.position = screenPos;
			}

			wasOnScreenLastFrame = isOnScreen;

		}

		ToggleVisibility(shouldDisplay);

	}

	public void SetScreenExitCallback(Action<UiElement, bool> callback)
	{
		onScreenExit = callback;
	}

	public void SetScreenEnterCallback(Action<UiElement> callback)
	{
		onScreenEnter = callback;
	}

	public void SetWorldSpaceTracking(bool state, Transform worldTarget, bool tracksOffScreen)
	{
		trackWorldPosition = state;
		this.worldTarget = worldTarget;
		this.tracksWhileOffScreen = tracksOffScreen;
	}

	public void SetWorldSpaceTracking(bool state)
	{
		trackWorldPosition = state;		
	}

	/// <summary>
	/// Change the visibility of the ui element outside of it's own class
	/// </summary>
	/// <param name="state"></param>
	public void SetVisibilityState(bool state)
	{
		shouldDisplay = state;
	}

	public void SetScaling(bool scalesWithDistance, float minScale, float maxScale, float maxDistance, float distanceBeforeDownscaling)
	{
		this.scalesWithDistance = scalesWithDistance;
		this.minScale = minScale;
		this.maxScale = maxScale;
		this.maxDistance = maxDistance;
		this.distanceBeforeDownscaling = distanceBeforeDownscaling; 
	}

	private void ToggleVisibility(bool state)
	{
		if (state == true)
		{
			EnableUiElement();
		}
		else
		{
			DisableUiElement();
		}
	}

	//Overriden by subclasses for custom behaviour
	protected virtual void DisableUiElement()
	{
		uiElement.SetActive(false);
	}

	protected virtual void EnableUiElement()
	{
		uiElement.SetActive(true);
	}

	public virtual void DestroyUiElement()
	{
		onUiElementDestroyed?.Invoke(this);
		Destroy(this.gameObject);
	}

	public void SetPositionOffset(Vector3 offset)
	{
		worldPositionOffset = offset;
	}



}
