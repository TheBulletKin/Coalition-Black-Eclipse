using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HiddenStatusEffect : IStatusEffect
{
	public void ApplyEffect(ControllableEntity entity)
	{
		EntityVisibility visibility = entity.GetComponent<EntityVisibility>();
		if (visibility != null)
		{
			visibility.SetVisibilityModifier(0);
		}
	}

	public void RemoveEffect(ControllableEntity entity)
	{
		EntityVisibility visibility = entity.GetComponent<EntityVisibility>();
		if (visibility != null)
		{
			visibility.SetVisibilityModifier(1);
		}
	}

	Image IStatusEffect.GetStatusIcon()
	{
		return null;
	}

	string IStatusEffect.GetStatusName()
	{
		return "Hidden";
	}
}
