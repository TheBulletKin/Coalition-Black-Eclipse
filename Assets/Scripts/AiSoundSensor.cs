using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiSoundSensor : MonoBehaviour
{
	[SerializeField] private AiHearingConfig hearingConfig;
	public bool hasHeardSound;
	public Sound soundHeard;
	public float timeSinceSoundHeard = 0f;
	public float investigateTimeLimit = 5f;

	private void Update()
	{
		if (hasHeardSound)
		{
			timeSinceSoundHeard += Time.deltaTime;
		}
		else
		{
			timeSinceSoundHeard = 0f;
		}
	}

	public void TestSound(Sound sound)
	{
		//If sound is beyond hearing range
		if (Vector3.Distance(transform.position, sound.soundPos) > hearingConfig.hearingRadius)
		{
			return;
		}

		if (sound.isOccluded)
		{
			//Linecast asks for a start point and end point, not direction#
			//If sound is blocked
			if (Physics.Linecast(sound.soundPos, transform.position, sound.occludedLayers))
			{
				return;
			}
		}

		ReactToSound(sound);
	}

	public void ReactToSound(Sound sound)
	{
		hasHeardSound = true;
		soundHeard = sound;
	}

	public void ForgetSound()
	{
		hasHeardSound = false;
		soundHeard = default;
	}
}
