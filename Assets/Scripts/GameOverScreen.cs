using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
	public TextMeshProUGUI gameOverText;
	private void Start()
	{
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		switch (GameManager.Instance.getEndReason())
		{
			case EndReason.OBJECTIVE_COMPLETE:
				gameOverText.text = "Objectives complete, you won!";
				gameOverText.color = Color.green;
				break;
			case EndReason.TIME_ENDED:
				break;
			case EndReason.ALL_TEAM_DEAD:
				gameOverText.text = "All teammates dead, you lost";
				gameOverText.color = Color.red;
				break;
			case EndReason.MANUAL_CLOSE:
				break;
			case EndReason.IN_PROGRESS:
				break;
			default:
				break;
		}
	}
}
