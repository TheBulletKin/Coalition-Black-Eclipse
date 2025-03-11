using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairBar : MonoBehaviour
{
    public CrosshairPositionType barPosition;
    public RectTransform rectTransform;
	public Vector3 defaultPosition;

	private void Start()
	{
		rectTransform = GetComponent<RectTransform>();
		defaultPosition = rectTransform.localPosition;
	}
}
