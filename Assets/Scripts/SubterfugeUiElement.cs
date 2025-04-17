using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubterfugeUiElement : UiElement, IGadgetUiElement
{
	[SerializeField] private Image disguiseTimerFill;
	public EntityVisibility visibility;
	public ControllableEntity entity;
	public LayoutElement layoutElement;

	private void Update()
	{
		if (!gameObject.activeInHierarchy || !uiElement.activeInHierarchy) return;

		if (visibility != null && visibility.GetVisibilityMod() < 1)
		{
			float remaining = visibility.GetHiddenTimeRemaining();
			float total = visibility.GetHiddenDuration();
			disguiseTimerFill.fillAmount = remaining / total;
		}
		else
		{
			disguiseTimerFill.fillAmount = 0f;
		}
	}
	public void OnGadgetActivated(IGadget gadget)
	{
		
	}

	public void OnGadgetDeactivated(IGadget gadget)
	{
		
	}

	public void OnGadgetDestroyed(IGadget gadget)
	{
		Destroy(gameObject);
	}

	public void OnGadgetPlaced(IGadget gadget)
	{
		if (gadget is SubterfugeAbility subterfuge)
		{

		}
	}

	public bool IsInHiddenDuration()
	{
		return visibility.IsHidden();
	}
}
