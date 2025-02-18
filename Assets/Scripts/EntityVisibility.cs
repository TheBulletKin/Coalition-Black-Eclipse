using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityVisibility : MonoBehaviour
{
    [SerializeField] private float visibilityModifier;

	private void Start()
	{
        visibilityModifier = 1.0f;
	}

	public float GetVisibilityMod()
    {
        return visibilityModifier;
    }

    public void ChangeVisibilityModifier(float newVis)
    {
        visibilityModifier = newVis;
    }
}
