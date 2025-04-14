using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Assets for single clip sounds
[CreateAssetMenu(menuName = "Audio/Sound Single")]
public class GameSoundSingle : GameSound
{
    public AudioClip clip;

	public override AudioClip GetAudioClip()
	{
		return clip;
	}
}
