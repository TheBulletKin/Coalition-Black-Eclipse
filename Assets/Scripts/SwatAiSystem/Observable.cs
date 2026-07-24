using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observable : MonoBehaviour
{
    public ObservableType type;
}

public enum ObservableType
{
    ENEMY,
    ROOM_MEMORYNODE,
    THRESHOLD_MEMORYNODE
}
