using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityVisibility : MonoBehaviour
{
    [SerializeField] private float visibilityModifier;

    public float GetVisibilityMod()
    {
        return visibilityModifier;
    }

    public void ChangeVisibilityModifier()
    {

    }
}
