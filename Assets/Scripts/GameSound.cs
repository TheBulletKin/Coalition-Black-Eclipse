using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base type for game sounds. Holds configuration values
/// </summary>
public abstract class GameSound : ScriptableObject
{   
    public float volume = 1f;
    public float pitch = 1f;
    public bool loop = false;
    public bool spatial = false;
	public float maxDistance = 50f;

    public abstract AudioClip GetAudioClip();   

    
}
