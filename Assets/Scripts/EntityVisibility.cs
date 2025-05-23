using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityVisibility : MonoBehaviour
{
	[SerializeField] private float visibilityModifier;
	private float hiddenDuration;
	private float hiddenTimer;
	public bool isDuration = false;
	public bool firingBreaksConcealment = true;	


	public Action durationCompleteCallback;

	//Used to apply
	public ControllableEntity entity;

	//Will hold all the things that can effect visibility for now, will change this later
	[SerializeField] public List<HiddenVeil> activeVeils = new List<HiddenVeil>();

	private void Start()
	{
		visibilityModifier = 1.0f;
		entity = GetComponent<ControllableEntity>();
	}

	private void Update()
	{
		if (isDuration)
		{
			hiddenTimer += Time.deltaTime;
			if (hiddenTimer > hiddenDuration)
			{
				CancelDuration(1);
			}
		}		
	}

	public float GetVisibilityMod()
	{
		return visibilityModifier;
	}

	public void SetVisibilityModifier(float newVis)
	{
		visibilityModifier = newVis;
	}

	public void HideForDuration(float duration, float visValue)
	{
		hiddenTimer = 0;
		hiddenDuration = duration;
		isDuration = true;
		SetVisibilityModifier(visValue);
	}

	public void SetDurationCompletionCallback(Action callback)
	{
		durationCompleteCallback = callback;
	}

	public bool IsHidden()
	{
		return visibilityModifier < 1;
	}

	public float GetHiddenTimeRemaining()
	{
		if (!isDuration)
		{
			return 0f;
		}

		return Mathf.Clamp(hiddenDuration - hiddenTimer, 0f, hiddenDuration);
	}

	public float GetHiddenDuration()
	{
		return hiddenDuration;
	}

	public void CancelDuration(float visValue)
	{
		hiddenTimer = 0;
		isDuration = false;	
		visibilityModifier = visValue;

		durationCompleteCallback?.Invoke();
	}

}
