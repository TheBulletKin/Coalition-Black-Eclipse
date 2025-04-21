using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IInitialisable
{

	public EnemyEntity[] enemies;


	
	public void Initialize()
	{	

		enemies = FindObjectsByType<EnemyEntity>(FindObjectsSortMode.InstanceID);

	}
    
}
