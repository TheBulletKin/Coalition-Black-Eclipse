using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class CharacterSwitcher : MonoBehaviour
{
    
	[SerializeField] private List<ControllableEntity> teammates;

	

	void Start()
	{
		InputManager.Instance.OnAiSwitchPressed += SwitchToCharacter;

		SwitchToCharacter(1);

	}

	public void SwitchToCharacter(int teammateId)
	{
		foreach (ControllableEntity teammate in teammates)
		{
			if (teammate.teammateID == teammateId)
			{
				/* Disable current player control related components
				 * Enable ai control components
				 * Enable current model
				 * Disable target's model
				 * 
				 */
				Debug.Log("Switched to " + teammateId);
			}
		}
	}
}
