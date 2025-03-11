using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyEntity[] enemies;
    // Start is called before the first frame update
    void Start()
    {
        enemies = FindObjectsByType<EnemyEntity>(FindObjectsSortMode.InstanceID);
    }

    
}
