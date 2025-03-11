using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour, IToggleable
{
	public float interactionRange = 3f;
	public LayerMask interactableLayer;
	public GameObject holdPoint;


	private void Start()
	{

	}

	private void Update()
	{

	}

	private void OnDestroy()
	{
		if (InputManager.Instance)
		{
			InputManager.Instance.OnInteractPressed -= TryInteract;
		}

	}

	private void TryInteract()
	{

		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
		RaycastHit hit;

		//Temporarily use this for ai spotting
		if (Physics.Raycast(ray, out hit, interactionRange, interactableLayer))
		{
			IInteractable interactable = hit.collider.GetComponent<IInteractable>();
			if (interactable != null)
			{
				interactable.Interact(gameObject);
			}
		}


	}

	public void DisableControl()
	{
		InputManager.Instance.OnInteractPressed -= TryInteract;
		enabled = false;
	}

	public void EnableControl()
	{
		InputManager.Instance.OnInteractPressed += TryInteract;
		enabled = true;
	}
}
