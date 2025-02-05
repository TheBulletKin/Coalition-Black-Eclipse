using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
	public float interactionRange = 3f;
	public LayerMask interactableLayer;

	private void Start()
	{
	
		InputManager.Instance.OnInteractPressed += TryInteract;
	}

	private void Update()
	{

	}

	private void OnDestroy()
	{
		InputManager.Instance.OnInteractPressed -= TryInteract;
	}

	private void TryInteract()
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
		{
			IInteractable interactable = hit.collider.GetComponent<IInteractable>();
			if (interactable != null)
			{
				interactable.Interact(gameObject);
			}
		}
	}
}
