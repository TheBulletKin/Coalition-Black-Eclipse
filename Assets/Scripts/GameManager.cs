using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }
	[SerializeField] private GameState gameState;
	[SerializeField] private EndReason endReason = EndReason.IN_PROGRESS;

	[Header("Bootstrap classes")]
	[SerializeField] private InputManager inputManager;
	[SerializeField] private EnemyManager enemyManager;
	[SerializeField] private EntityManager entityManager;
	[SerializeField] private CameraStateSwitcher camStateSwitcher;
	[SerializeField] private PlayerCommandIssuer commandIssuer;
	[SerializeField] private EnemyMarkerManager enemyMarkerManager;
	[SerializeField] private CharacterSwitcher characterSwitcher;
	[SerializeField] private TeammateUiManager teammateUiManager;
	[SerializeField] private PlayerUiImanager playerUiImanager;
		

	// Start is called before the first frame update
	void Start()
    {		
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void InitializeGame()
	{
		/* 
		 * InputManager
		 * CameraStateSwitcher
		 * PlayerCommandIssuer
		 * EntityManager - (PlayerCommandIssuer, TeammateUiManager)
		 *		ControllableEntity
		 * PlayerUiManager - 
		 * TeammateUiManager - (PlayerCommandIssuer, PlayerUiManager)
		 * CharacterSwitcher - (InputManager, EntityManager, playerUiManager, cameraStateSwitcher)
		 * Enemy Manager 
		 * EnemyMarkerManager - (EnemyManager)
		 */


		inputManager.Initialize();
		camStateSwitcher.Initialize();
		commandIssuer.Initialize();
		entityManager.Initialize();
		characterSwitcher.Initialize();
		playerUiImanager.Initialize();
		teammateUiManager.Initialize();
		
		enemyManager.Initialize();
		enemyMarkerManager.Initialize();

		gameState = GameState.RUNNING;
		Debug.Log("Game Initialized.");
	}

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;			
		}
		else
		{
			Destroy(gameObject);
			return;
		}		

		InitializeGame();
	}

	public void FinishGame(EndReason reason)
	{
		endReason = reason;
		switch (reason)
		{
			case EndReason.OBJECTIVE_COMPLETE:
				Debug.Log("Game finished, objectives complete");
				SceneManager.LoadScene("GameOverScene");
				break;
			case EndReason.TIME_ENDED:
				break;
			case EndReason.ALL_TEAM_DEAD:
				Debug.Log("Game over, all teammates died");
				SceneManager.LoadScene("GameOverScene");
				
				break;
			case EndReason.MANUAL_CLOSE:
				break;
			default:
				break;
		}
	}

	public EndReason getEndReason()
	{
		return endReason;
	}
}
