using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;
	[SerializeField] private GameObject audioSourcePrefab;

	[Serializable]
	private class FootstepSounds
	{
		public List<AudioClip> clips;
	}

	[SerializeField] private FootstepSounds footstepSounds;

	[Serializable]
	private class IdleBarkSounds
	{
		public List<AudioClip> clips;
	}

	[SerializeField] private IdleBarkSounds idleBarkSounds;

	public Dictionary<SoundType, List<AudioClip>> soundTypeToAudioClip = new Dictionary<SoundType, List<AudioClip>>();
	[Tooltip("All currently active audio sources")]
	public List<AudioSource> sources = new List<AudioSource>();

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

	private void Start()
	{
		soundTypeToAudioClip.Add(SoundType.FOOTSTEP, footstepSounds.clips);
		soundTypeToAudioClip.Add(SoundType.IDLE_BARK, idleBarkSounds.clips);
	}

	/// <summary>
	/// Uses a simple object pooling approach. If an audio source has been created and isn't playing anything, it's
	/// sound is replaced and played from the start. This avoids constantly creating and deleting. Will only create a new object
	/// if no existing sources are free
	/// </summary>
	/// <returns></returns>
	private AudioSource CreateAudioSource()
	{
		foreach (AudioSource activeSource in sources)
		{
			if (!activeSource.isPlaying)
			{
				return activeSource;
			}

		}

		GameObject sourceGameObject = Instantiate(audioSourcePrefab, transform);
		AudioSource source = sourceGameObject.GetComponent<AudioSource>();
		sources.Add(source);
		return source;
	}

	public void PlaySound(GameSound sound, Vector3? position = null, Transform followTarget = null)
	{
		Vector3 soundPos = followTarget ? followTarget.position : (position ?? Vector3.zero);
		Vector3 listenerPos = Camera.main.transform.position;

		float distance = Vector3.Distance(listenerPos, soundPos);
		if (sound.spatial && distance > sound.maxDistance)
		{
			return;
		}


		AudioSource source = CreateAudioSource();
		source.clip = sound.clip;
		source.volume = sound.volume;
		source.pitch = sound.pitch;
		source.loop = sound.loop;
		source.spatialBlend = sound.spatial ? 1f : 0f;

		if (followTarget != null)
		{
			source.transform.SetParent(followTarget);
			source.transform.localPosition = Vector3.zero;
		}
		else
		{
			source.transform.SetParent(null);
			source.transform.position = position ?? Vector3.zero;
		}

		source.Play();


	}

	public void StopSound(AudioSource source)
	{
		if (source != null)
		{
			source.Stop();
		}
	}

}
