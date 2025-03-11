using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : MonoBehaviour, IInteractable
{
	[SerializeField] public bool isRevealed = false;
	public void Interact(GameObject instigator)
	{
		isRevealed = true;
	}
}
