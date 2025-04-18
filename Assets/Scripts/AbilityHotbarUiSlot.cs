using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityHotbarUiSlot : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI iconText;
	[SerializeField] private TextMeshProUGUI keybindText;	
	[SerializeField] private Image abilityIcon;
	[SerializeField] private Image keybindImage;	
	
	[SerializeField] private GameObject abilityCount;
	[SerializeField] private Image countImage;
	[SerializeField] private TextMeshProUGUI countText;
	[SerializeField] private Color defaultColour;
	public bool isSelected;

	private void Start()
	{
		
	}

	public void ChangeHotbarSlotDetails(CharacterAbility ability)
	{
		iconText.text = ability.abilityName;
		if (ability.hasCountLimit)
		{
			abilityCount.SetActive(true);
			countText.text = ability.currentAbilityCount.ToString();
		}
		else
		{
			abilityCount.SetActive(false);
		}
	}

	public void ToggleHotbarSelection(bool state, Color selectionColour)
	{
		if (state)
		{
			abilityIcon.color = selectionColour;
		}
		else
		{
			abilityIcon.color = defaultColour;
		}
	}

	
}
