using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundEmitterHandler : MonoBehaviour
{
	public static SoundEmitterHandler instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;

		}
		else
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Sounds that the player can hear
	/// </summary>
	/// <param name="sound"></param>
	/// <param name="busTarget"></param>
	/// <param name="position"></param>
	/// <param name="followTarget"></param>
	public void EmitAudibleSound(GameSound sound, MixerBus busTarget, Vector3? position = null, Transform followTarget = null)
	{
		AudioManager.instance.PlaySound(sound, MixerBus.GUNSHOT, position, followTarget);
	}

	public void EmitAudibleSound(SoundType soundType, MixerBus busTarget, Vector3? position = null, Transform followTarget = null)
	{
		AudioManager.instance.PlaySound(soundType, MixerBus.GUNSHOT, position, followTarget);
	}

	/// <summary>
	/// Sounds that the ai can hear
	/// </summary>
	/// <param name="sound"></param>
	/// <param name="position"></param>
	public void EmitDetectableSound(Sound sound, Vector3 position)
	{
		sound.soundPos = position;
		Collider[] colliders = Physics.OverlapSphere(sound.soundPos, sound.soundRadius, sound.hearableLayers);
		foreach (var col in colliders)
		{
			AiSoundSensor soundSensor = col.GetComponentInParent<AiSoundSensor>();
			if (soundSensor != null)
			{				
				soundSensor.TestSound(sound);
			}
		}
	}
}
