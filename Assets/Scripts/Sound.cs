using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Sound
{
    [HideInInspector] public Vector3 soundPos;
    public float soundRadius;
    public bool isOccluded;
    public LayerMask occludedLayers;
	public LayerMask hearableLayers;
}
