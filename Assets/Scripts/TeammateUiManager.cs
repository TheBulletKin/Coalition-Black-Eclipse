using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TeammateUiManager : MonoBehaviour
{
	[SerializeField] private PlayerCommandIssuer player;
	private Dictionary<AiCommandListener, UiTeammateCard> teammateToUiCard = new Dictionary<AiCommandListener, UiTeammateCard>();
	private Dictionary<AiCommandListener, UiTeammateWorldMarker> teammateToUiNameplate = new Dictionary<AiCommandListener, UiTeammateWorldMarker>();
	[SerializeField] private GameObject teammateUiCardPrefab;
	[SerializeField] private GameObject teammateUiTemplatePrefab;
	[SerializeField] private RectTransform nameplatePanel;
	[SerializeField] private RectTransform cardsContainer;
	[SerializeField] private float nameplateHeightOffset = 2f;
	[SerializeField] private float offScreenElementRadiusPercentage = 0.75f;
	private float floatingUiElementRadius;
	[SerializeField] private float offScreenElementScale = 0.5f;
	[SerializeField] private float minScale = 0.5f;
	[SerializeField] private float maxScale = 1.0f;
	[SerializeField] private float maxDistance = 50f;
	[SerializeField] private float distanceBeforeDownscaling = 10f;

	private List<UiElement> teammateWorldMarkerElements = new List<UiElement>();

	private void Start()
	{
		//When off screen, ui elements will form a ring around the screen centre. This sets that radius based on screen size
		floatingUiElementRadius = Mathf.Min(Screen.width, Screen.height) * offScreenElementRadiusPercentage;

		List<AiCommandListener> aiTeammates = player.GetTeammates();
		if (aiTeammates != null)
		{
			foreach (AiCommandListener ai in aiTeammates)
			{
				//---- Onscreen teammate cards
				//Create the ui card game objects for all teammates present on player
				GameObject newTeammateCardObject = Instantiate(teammateUiCardPrefab, cardsContainer.transform.position, cardsContainer.transform.rotation, cardsContainer);
				UiTeammateCard teammateCard = newTeammateCardObject.GetComponent<UiTeammateCard>();
				teammateCard.teammateColour = ai.teammateColour;
				teammateCard.background.color = ai.teammateColour;
				teammateCard.teamIndexText.text = (aiTeammates.IndexOf(ai) + 1).ToString();
				teammateCard.nameText.text = ai.teammateName;

				teammateCard.SetVisibilityState(true);				

				teammateToUiCard.Add(ai, teammateCard);

				//----Teammate world markers
				//Create the ui nameplates that follow teammates
				GameObject newTeammateWorldMarkerObject = Instantiate(teammateUiTemplatePrefab, gameObject.transform.position, gameObject.transform.rotation, nameplatePanel);
				UiTeammateWorldMarker teammateWorldMarker = newTeammateWorldMarkerObject.GetComponent<UiTeammateWorldMarker>();
				teammateWorldMarker.teammateColour = ai.teammateColour;
				teammateWorldMarker.background.color = ai.teammateColour;
				teammateWorldMarker.teamIndexText.text = (aiTeammates.IndexOf(ai) + 1).ToString();

				teammateWorldMarker.SetVisibilityState(true);
				teammateWorldMarker.SetWorldSpaceTracking(true, ai.transform, true);
				teammateWorldMarker.SetScreenExitCallback(HandleScreenExit);
				teammateWorldMarker.SetScreenEnterCallback(HandleScreenEnter);
				teammateWorldMarker.SetPositionOffset(new Vector3(0, nameplateHeightOffset, 0));
				teammateWorldMarker.offScreenFloatingRadius = floatingUiElementRadius;
				teammateWorldMarker.SetScaling(true, minScale, maxScale, maxDistance, distanceBeforeDownscaling);

				teammateToUiNameplate.Add(ai, teammateWorldMarker);
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
		
	}

	private void HandleScreenEnter(UiElement element)
	{
		element.SetScaling(true, minScale, maxScale, maxDistance, distanceBeforeDownscaling);
		element.SetVisibilityState(true);
		element.uiElementRectTransform.localScale = Vector3.one;
	}


	private void HandleScreenExit(UiElement element, bool tracksWhileOffScreen)
	{
		if (tracksWhileOffScreen)
		{
			element.SetScaling(false, 1.0f, 1.0f, 10f, 5);
			element.uiElementRectTransform.localScale = new Vector3(offScreenElementScale, offScreenElementScale, offScreenElementScale);
		}
		else
		{			
			element.SetVisibilityState(false);
		}

	}
}
