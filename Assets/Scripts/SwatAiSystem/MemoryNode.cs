using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryNode : MonoBehaviour
{
    public float clearFactor = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        clearFactor = 0.0f;

	}

    // Update is called once per frame
    void Update()
    {
        //If looked at, raise clear factor
    }
}

public enum MemoryNodeType
{
    ROOM_LOCATION,
    THRESHOLD
}
