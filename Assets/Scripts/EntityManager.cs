using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
	public static EntityManager Instance { get; private set; }

	//Public for now, alter later
	public List<ControllableEntity> playerTeammates;
	public List<Decoy> playerDecoys { get; private set; }

	private CharacterSwitcher characterSwitcher;
	private TeammateUiManager teammateUiManager;





	private void Start()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}

		playerDecoys = new List<Decoy>();

		ControllableEntity[] foundTeammates = FindObjectsByType<ControllableEntity>(FindObjectsSortMode.InstanceID);
		foreach (ControllableEntity teammate in foundTeammates)
		{
			Instance.playerTeammates.Add(teammate);
			teammate.health.OnEntityDeath += EntityDeathResponse;
		}

		characterSwitcher = FindObjectOfType<CharacterSwitcher>();
		teammateUiManager = FindObjectOfType<TeammateUiManager>();

	}

	public void AddNewDecoy(Decoy decoy)
	{
		Instance.playerDecoys.Add(decoy);
		decoy.health.OnEntityDeath += EntityDeathResponse;
	}

	public void EntityDeathResponse(Health entity)
	{
		//Using getcomponent for now. Will change later
		entity.OnEntityDeath -= EntityDeathResponse;

		//If the killed entity was a playable teammate
		ControllableEntity controllableEntity = entity.GetComponent<ControllableEntity>();
		if (controllableEntity != null)
		{
			controllableEntity.HandleDeath();

			Instance.playerTeammates.Remove(controllableEntity);

			//Automatically switch to random character. (Will set up what happens when all are dead later)
			if (controllableEntity.teammateID == characterSwitcher.currentlyControlledTeammate)
			{
				int divertedCharacterIndex = Random.Range(0, playerTeammates.Count);
				characterSwitcher.SwitchToCharacter(playerTeammates[divertedCharacterIndex]);
			}

			//Remove the ui elements for that teammate
			teammateUiManager.RemoveTeammateCard(controllableEntity.commandListener);
		}

		//If the killed entity was a player created decoy
		Decoy decoyEntity = entity.GetComponent<Decoy>();
		if (decoyEntity != null)
		{
			Instance.playerDecoys.Remove(decoyEntity);
		}

	}

}
