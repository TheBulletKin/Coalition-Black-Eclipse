using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IStatusEffect
{
    //Temporarily set to teammate entities for now. Will improve to fit all types later
    public void ApplyEffect(ControllableEntity entity);
	public void RemoveEffect(ControllableEntity entity);
    public string GetStatusName();
    public Image GetStatusIcon();
}
