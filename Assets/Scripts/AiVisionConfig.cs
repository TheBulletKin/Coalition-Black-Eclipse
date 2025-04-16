using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewVisionConfig", menuName = "Ai Senses/Vision Config")]
public class AiVisionConfig : ScriptableObject
{
	[SerializeField] private float detectionIncreaseRate = 5f;
	[SerializeField] private float detectionDecreaseRate = 2f;
	[SerializeField] private float detectionThreshold = 100f;

	[SerializeField] private float detectionAngle = 60f;
	[SerializeField] private float maxDetectionDistance = 40f;
	[SerializeField] private float closeDetectionFalloffDistance = 10f;
	[SerializeField] private float detectionRateModifier;
}
