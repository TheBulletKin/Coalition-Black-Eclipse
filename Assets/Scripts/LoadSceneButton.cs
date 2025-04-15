using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MainMenuButton
{
	public string SceneName;
	protected override void OnClick()
	{
		SceneManager.LoadScene(SceneName);
	}
	
}
