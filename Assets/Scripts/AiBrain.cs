using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiBrain : MonoBehaviour
{
    public AiSightSensor sightSensor;
    public AiSoundSensor soundSensor;
    public AIMovement aiMovement;

	private void Start()
	{
		sightSensor = GetComponent<AiSightSensor>();
		soundSensor = GetComponent<AiSoundSensor>();
		aiMovement = GetComponent<AIMovement>();
	}
}
