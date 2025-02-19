using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleProjectile : MonoBehaviour
{
	public bool isThrown = false;
	private void OnCollisionEnter(Collision collision)
	{
		
		if (isThrown)
		{
			Destroy(gameObject);
		}
	}
}
