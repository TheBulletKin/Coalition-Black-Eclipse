using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeammateUiManager : MonoBehaviour
{
	public PlayerCommandIssuer player;
	public Dictionary<AiCommandListener, UiTeammateCard> teammateToUiCard = new Dictionary<AiCommandListener, UiTeammateCard>();
	public GameObject teammateUiCardPrefab;

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
}
