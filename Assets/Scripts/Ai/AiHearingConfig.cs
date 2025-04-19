using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHearingConfig", menuName = "Ai Senses/Hearing Config")]
public class AiHearingConfig : ScriptableObject
{
    public float hearingRadius = 20f;
	public float investigateTimeLimit = 5f;
}
