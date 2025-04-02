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

	public void ChangeHotbarSlotDetails(CharacterAbility ability)
	{
		iconText.text = ability.abilityName;
	}

	
}
