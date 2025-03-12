using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TeammateUiManager : MonoBehaviour
{
	[SerializeField] private PlayerCommandIssuer player;
	private Dictionary<AiCommandListener, UiTeammateCard> teammateToUiCard = new Dictionary<AiCommandListener, UiTeammateCard>();
	private Dictionary<AiCommandListener, UiTeammateNameplate> teammateToUiNameplate = new Dictionary<AiCommandListener, UiTeammateNameplate>();
	[SerializeField] private GameObject teammateUiCardPrefab;
	[SerializeField] private GameObject teammateUiTemplatePrefab;
	[SerializeField] private RectTransform nameplatePanel;
	[SerializeField] private RectTransform cardsContainer;
	[SerializeField] private float nameplateHeightOffset = 2f;

	private void Start()
	{
		List<AiCommandListener> aiTeammates = player.GetTeammates();
		if (aiTeammates != null)
		{
			foreach (AiCommandListener ai in aiTeammates)
			{
				//Create the ui card game objects for all teammates present on player
				GameObject newTeammateCardObject = Instantiate(teammateUiCardPrefab, cardsContainer.transform.position, cardsContainer.transform.rotation, cardsContainer);
				UiTeammateCard teammateCard = newTeammateCardObject.GetComponent<UiTeammateCard>();
				teammateCard.teammateColour = ai.teammateColour;
				teammateCard.background.color = ai.teammateColour;
				teammateCard.teamIndexText.text = (aiTeammates.IndexOf(ai) + 1).ToString();
				teammateToUiCard.Add(ai, teammateCard);

				//Create the ui nameplates that follow teammates
				GameObject newTeammateNameplateObject = Instantiate(teammateUiTemplatePrefab, gameObject.transform.position, gameObject.transform.rotation, nameplatePanel);
				UiTeammateNameplate teammateNameplate = newTeammateNameplateObject.GetComponent<UiTeammateNameplate>();
				teammateNameplate.teammateColour = ai.teammateColour;
				teammateNameplate.background.color = ai.teammateColour;
				teammateNameplate.teamIndexText.text = (aiTeammates.IndexOf(ai) + 1).ToString();
				teammateToUiNameplate.Add(ai, teammateNameplate);
			}

			player.OnTeammateOrGroupChanged += SwitchActiveCard;
		}
		else
		{
			Debug.LogError("Ai Teammates currently null on Start");
		}

	}

	/// <summary>
	/// Disables all cards and enables the ui elements for teammates matching the index passed in
	/// </summary>
	/// <param name="teammateOrGroupIndex"></param>
	/// <param name="isGroup"></param>
	public void SwitchActiveCard(int teammateOrGroupIndex, bool isGroup)
	{
		foreach (KeyValuePair<AiCommandListener, UiTeammateCard> pair in teammateToUiCard)
		{
			//Disable all teammate cards and nameplates
			pair.Value.ToggleAsActiveTeammate(false);
			teammateToUiNameplate[pair.Key].ToggleAsActiveTeammate(false);

			//If a group was selected
			if (isGroup && pair.Key.groupIndex == teammateOrGroupIndex)
			{
				UiTeammateCard activeCard = pair.Value;
				activeCard.ToggleAsActiveTeammate(true);

				teammateToUiNameplate[pair.Key].ToggleAsActiveTeammate(true);
			}
			//If it's just an individual selected
			else if (!isGroup && teammateOrGroupIndex == player.GetTeammates().IndexOf(pair.Key))
			{
				UiTeammateCard activeCard = pair.Value;
				activeCard.ToggleAsActiveTeammate(true);

				teammateToUiNameplate[pair.Key].ToggleAsActiveTeammate(true);
			}
		}		
	}

	public void RemoveTeammateCard(AiCommandListener teammate)
	{
		Destroy(teammateToUiCard[teammate].gameObject);
		Destroy(teammateToUiNameplate[teammate].gameObject);
		teammateToUiCard.Remove(teammate);
		teammateToUiNameplate.Remove(teammate);
	}

	private void Update()
	{
		foreach (KeyValuePair<AiCommandListener, UiTeammateNameplate> pair in teammateToUiNameplate)
		{
			//For every ai unit in world space, convert to screen space and position the UI element on the player's Ui
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
