using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }
	[SerializeField] private GameState gameState;

	[SerializeField] private GameObject victoryMessage;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void Awake()
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

		gameState = GameState.RUNNING;
	}

	public void FinishGame(EndReason reason)
	{
		switch (reason)
		{
			case EndReason.OBJECTIVE_COMPLETE:
				Debug.Log("Game finished, objectives complete");
				victoryMessage.SetActive(true);
				break;
			case EndReason.TIME_ENDED:
				break;
			case EndReason.ALL_TEAM_DEAD:
				break;
			case EndReason.MANUAL_CLOSE:
				break;
			default:
				break;
		}
	}
}
