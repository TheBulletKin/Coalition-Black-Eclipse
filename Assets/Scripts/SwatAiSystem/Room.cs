using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<MemoryNode> roomNodes;

    [Serializable]
    private class RoomPathNodeCollection
    {
        public int teammateIndex;
        public List<RoomPathNode> roomPathNodes;
    }

    [SerializeField]
    private List<RoomPathNodeCollection> roomPathNodeCollections;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
