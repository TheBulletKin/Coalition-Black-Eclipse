using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Asset type for sound collections.
/// NOTE: Currenttly can only return a random sound from the collection.
/// </summary>
[CreateAssetMenu(menuName = "Audio/Sound Collection")]

public class GameSoundCollection : GameSound
{
	public List<AudioClip> clips;

	public override AudioClip GetAudioClip()
	{
		AudioClip sound = clips[UnityEngine.Random.Range(0, clips.Count)];
		return sound;
	}
}
