using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;
	[SerializeField] private GameObject audioSourcePrefab;
	[SerializeField] private AudioMixer mainAudioMixer;
	[SerializeField] private AudioMixerGroup playerFootstepGroup;
	[SerializeField] private AudioMixerGroup enemyFootstepGroup;
	[SerializeField] private AudioMixerGroup musicGroup;


	[Serializable]
	private class SoundCollections
	{
		public GameSoundCollection footstepSounds;
		public GameSoundCollection idleBarkSounds;
	}

	[SerializeField]
	private SoundCollections soundCollections;

	public GameSoundSingle backgroundMusic;

	public Dictionary<SoundType, GameSound> soundTypeToAudioClip = new Dictionary<SoundType, GameSound>();
	[Tooltip("All currently active audio sources")]
	public List<AudioSource> sources = new List<AudioSource>();

	private int maxPoolSize = 25;

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
		soundTypeToAudioClip.Add(SoundType.FOOTSTEP, soundCollections.footstepSounds);
		soundTypeToAudioClip.Add(SoundType.IDLE_BARK, soundCollections.idleBarkSounds);
		soundTypeToAudioClip.Add(SoundType.MUSIC, backgroundMusic);

		PlaySound(SoundType.MUSIC, MixerBus.MUSIC, null, null);
	}

	/// <summary>
	/// Uses a simple object pooling approach. If an audio source has been created and isn't playing anything, it's
	/// sound is replaced and played from the start. This avoids constantly creating and deleting. Will only create a new object
	/// if no existing sources are free
	/// </summary>
	/// <returns></returns>
	private AudioSource CreateAudioSource()
	{
		
		//If an audio source is free, use that.
		//if the audio source has been deleted for whatever reason, ensure it's removed from the list
		for (int i = sources.Count - 1; i >= 0; i--)
		{
			AudioSource activeSource = sources[i];
			
			if (activeSource == null || !activeSource.isPlaying)
			{
				sources.RemoveAt(i);
			}
			else if (!activeSource.isPlaying)
			{
				return activeSource;
			}
		}

		if (sources.Count >= maxPoolSize)
		{
			Debug.LogWarning("AudioSource pool is full, no new sources can be created");
			return null;
		}

		GameObject sourceGameObject = Instantiate(audioSourcePrefab, transform);
		AudioSource source = sourceGameObject.GetComponent<AudioSource>();
		sources.Add(source);
		return source;
	}

	public void PlaySound(SoundType type, MixerBus busTarget, Vector3? position = null, Transform followTarget = null)
	{
		Vector3 soundPos = followTarget ? followTarget.position : (position ?? Vector3.zero);
		Vector3 listenerPos = Camera.main.transform.position;

		GameSound sound = soundTypeToAudioClip[type];
		if (sound == null)
		{
			Debug.LogWarning("Sound not found");
			return;
		}

		float distance = Vector3.Distance(listenerPos, soundPos);
		if (sound.spatial && distance > sound.maxDistance)
		{
			return;
		}



		AudioSource source = CreateAudioSource();
		if (source == null)
		{
			Debug.LogWarning("No available audio sources to play sound");
			return;
		}

		source.clip = sound.GetAudioClip();
		source.volume = sound.volume;
		source.pitch = sound.pitch;
		source.loop = sound.loop;
		source.spatialBlend = sound.spatial ? 1f : 0f;

		switch (busTarget)
		{
			case MixerBus.FOOTSTEP_PLAYER:
				source.outputAudioMixerGroup = playerFootstepGroup;
				break;
			case MixerBus.FOOTSTEP_ENEMY:
				source.outputAudioMixerGroup = enemyFootstepGroup;
				break;
			case MixerBus.MUSIC:
				source.outputAudioMixerGroup = musicGroup;
				break;
		}

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
