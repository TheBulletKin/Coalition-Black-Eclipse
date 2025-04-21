using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour, IInitialisable
{
	public static EntityManager Instance { get; private set; }

	//Public for now, alter later
	public List<ControllableEntity> playerTeammates;
	public List<Decoy> playerDecoys { get; private set; }

	private CharacterSwitcher characterSwitcher;
	private TeammateUiManager teammateUiManager;



	/// <summary>
	/// Requires: CharacterSwitcher, TeammateUiManager
	/// </summary>
	/// <returns></returns>
	public void Initialize()
	{	

		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		playerDecoys = new List<Decoy>();

		ControllableEntity[] foundTeammates = FindObjectsByType<ControllableEntity>(FindObjectsSortMode.InstanceID);
		foreach (ControllableEntity teammate in foundTeammates)
		{
			teammate.Initialize();
			Instance.playerTeammates.Add(teammate);
			teammate.health.OnEntityDeath += EntityDeathResponse;
			
		}
		
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

			if (Instance.playerTeammates.Count == 0) //When all teammates dead
			{
				GameManager.Instance.FinishGame(EndReason.ALL_TEAM_DEAD);
				return;
			}

			if (characterSwitcher == null)
			{
				characterSwitcher = FindObjectOfType<CharacterSwitcher>();
			}

			//Automatically switch to random character. (Will set up what happens when all are dead later)
			if (playerTeammates.Count > 0 && controllableEntity.teammateID == characterSwitcher.currentlyControlledTeammate)
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
