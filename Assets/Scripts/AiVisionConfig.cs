using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewVisionConfig", menuName = "Ai Senses/Vision Config")]
public class AiVisionConfig : ScriptableObject
{
	public float detectionIncreaseRate = 5f;
	public float detectionDecreaseRate = 2f;
	public float detectionThreshold = 100f;

	public float detectionAngle = 60f;
	public float preferredAngle = 20f;
	public float maxDetectionDistance = 40f;
	public float closeDetectionFalloffDistance = 10f;
	public float detectionRateModifier;

	public LayerMask ignoreMask;
}
