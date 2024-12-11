using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TeammateUiManager : MonoBehaviour
{
	public PlayerCommandIssuer player;
	public Dictionary<AiCommandListener, UiTeammateCard> teammateToUiCard = new Dictionary<AiCommandListener, UiTeammateCard>();
	public Dictionary<AiCommandListener, UiTeammateNameplate> teammateToUiTemplate = new Dictionary<AiCommandListener, UiTeammateNameplate>();
	public GameObject teammateUiCardPrefab;
	public GameObject teammateUiTemplatePrefab;
	public RectTransform nameplatePanel;
	public float nameplateHeightOffset = 2f;

	private void Start()
	{
		List<AiCommandListener> aiTeammates = player.GetTeammates();
		if (aiTeammates != null)
		{
			foreach (AiCommandListener ai in aiTeammates)
			{
				GameObject newTeammateCardObject = Instantiate(teammateUiCardPrefab, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
				UiTeammateCard teammateCard = newTeammateCardObject.GetComponent<UiTeammateCard>();
				teammateCard.teammateColour = ai.teammateColour;
				teammateCard.background.color = ai.teammateColour;
				teammateCard.teamIndexText.text = aiTeammates.IndexOf(ai).ToString();
				teammateToUiCard.Add(ai, teammateCard);

				GameObject newTeammateNameplateObject = Instantiate(teammateUiTemplatePrefab, gameObject.transform.position, gameObject.transform.rotation, nameplatePanel);
				UiTeammateNameplate teammateNameplate = newTeammateNameplateObject.GetComponent<UiTeammateNameplate>();
				teammateNameplate.teammateColour = ai.teammateColour;
				teammateNameplate.background.color = ai.teammateColour;
				teammateNameplate.teamIndexText.text = aiTeammates.IndexOf(ai).ToString();
				teammateToUiTemplate.Add(ai, teammateNameplate);
			}

			player.OnTeammateOrGroupChanged += SwitchActiveCard;
		}
		else
		{
			Debug.LogError("Ai Teammates currently null on Start");
		}

	}

	public void SwitchActiveCard(int teammateOrGroupIndex, bool isGroup)
	{
		foreach (KeyValuePair<AiCommandListener, UiTeammateCard> pair in teammateToUiCard)
		{
			pair.Value.ToggleAsActiveTeammate(false);

			if (isGroup && pair.Key.groupIndex == teammateOrGroupIndex)
			{
				UiTeammateCard activeCard = pair.Value;
				activeCard.ToggleAsActiveTeammate(true);
			}
		}

		if (!isGroup)
		{
			UiTeammateCard activeCard = teammateToUiCard[player.GetTeammates()[teammateOrGroupIndex]];
			activeCard.ToggleAsActiveTeammate(true);
		}



	}

	private void Update()
	{

		foreach (KeyValuePair<AiCommandListener, UiTeammateNameplate> pair in teammateToUiTemplate)
		{
			Vector3 screenPosition = Camera.main.WorldToScreenPoint(pair.Key.transform.position + Vector3.up * nameplateHeightOffset);
			screenPosition.x = Mathf.Clamp(screenPosition.x, 0, Screen.width);
			screenPosition.y = Mathf.Clamp(screenPosition.y, 0, Screen.height);

			
			if (screenPosition.z < 0)
			{
				pair.Value.gameObject.SetActive(false);
			}
			else
			{
				pair.Value.gameObject.SetActive(true);				
				pair.Value.transform.position = screenPosition;
			}
		}
		

	}
}
