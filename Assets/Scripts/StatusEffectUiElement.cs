using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectUiElement : UiElement
{
    public TextMeshProUGUI text;
    public Image icon;
    public ControllableEntity relatedEntity;
    public LayoutElement layoutElement;

	private void Start()
	{
		layoutElement = GetComponent<LayoutElement>();
	}
}
