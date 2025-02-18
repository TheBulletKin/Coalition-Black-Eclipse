using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximitySensorObject : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Enemy"))
		{
			Debug.Log("Enemy entered prox sensor range");
		}
		if (other.gameObject.CompareTag("Player"))
		{
			Debug.Log("Player entered prox sensor range");
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Enemy"))
		{
			Debug.Log("Enemy left prox sensor range");
		}
		if (other.gameObject.CompareTag("Player"))
		{
			Debug.Log("Player left prox sensor range");
		}
	}
}
