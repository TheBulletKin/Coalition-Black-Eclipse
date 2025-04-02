using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenVeil : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		
		if (other.gameObject.CompareTag("Teammate"))
		{
			other.gameObject.GetComponent<EntityVisibility>().ChangeVisibilityModifier(0);
			Debug.Log(other.gameObject.name + " entered veil");
		}
	}

	private void OnTriggerExit(Collider other)
	{
		
		if (other.gameObject.CompareTag("Teammate"))
		{
			other.gameObject.GetComponent<EntityVisibility>().ChangeVisibilityModifier(1);
			Debug.Log(other.gameObject.name + " exited veil");
		}
	}
}
