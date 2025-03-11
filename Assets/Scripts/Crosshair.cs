using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] public List<CrosshairBar> movingCrosshairParts = new List<CrosshairBar>();
	
	

	private void Start()
	{		
	}

	
	public void UpdateSpreadVisual(float currentBaseSpread)
    {
		
		foreach (CrosshairBar item in movingCrosshairParts)
		{
			//This method is called in update, so this is a live representation of the current spread value
						
			Vector2 crosshairBarPos = item.defaultPosition + item.rectTransform.localPosition;

			//Determine the direction to move it in
			Vector2 newCrosshairBarPos;
			switch (item.barPosition) //Change the direction this will move in
			{
				case CrosshairPositionType.TOP:
					newCrosshairBarPos = Vector2.up;
					break;
				case CrosshairPositionType.RIGHT:
					newCrosshairBarPos = Vector2.right;
					break;
				case CrosshairPositionType.LEFT:
					newCrosshairBarPos = Vector2.left;
					break;
				case CrosshairPositionType.BOTTOM:
					newCrosshairBarPos = Vector2.down;
					break;
				default:
					newCrosshairBarPos = Vector2.up;
					break;
			}
			//Now width of base spread
			newCrosshairBarPos *= (currentBaseSpread + 24f);

			
			item.transform.localPosition = new Vector3(newCrosshairBarPos.x, newCrosshairBarPos.y, 0.0f);
		}
    }


}
