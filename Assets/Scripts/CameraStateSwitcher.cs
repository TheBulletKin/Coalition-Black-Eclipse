using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStateSwitcher : MonoBehaviour
{
    public Transform playerCameraPos;
	public Vector3 currentPos;
	public Vector3 targetPos;
	public Quaternion prevCameraAngle;
    public Transform topDownCameraPos;
	public CameraStates currentState;
	
	public float mapCamDistance = 25f;
	private bool isMapView = false;

	public PlayerMovementController movementController;
	public CameraController cameraController;

	public static event Action<CameraStates> OnCameraStateChanged;


	/// <summary>
	/// Requires: InputManager
	/// </summary>
	/// <returns></returns>
	public void Initialize()
	{
		InputManager.Instance.OnMapViewEnterPressed += SwitchToMap;
		prevCameraAngle = Camera.main.transform.rotation;

	}


	private void Update()
	{
		if (isMapView)
		{
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = true;
		}
		else
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		
	}

	private void SwitchToMap()
	{
		//Toggle movement
		movementController.canMove = false;
		cameraController.canLookAround = false;

		isMapView = true;

		
		
		//Change position
		Camera.main.transform.position = topDownCameraPos.transform.position;

		//Cache old camera angle and rotate it downwards
		prevCameraAngle = Camera.main.transform.rotation;
		//Quaternion newRotation = Quaternion.Euler(90f, prevCameraAngle.y, prevCameraAngle.z);
		Quaternion newRotation = Quaternion.Euler(90f, 0f, 0f);
		Camera.main.transform.rotation = newRotation;

		//Confine cursor to window
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		Camera.main.orthographic = true;
		Camera.main.orthographicSize = mapCamDistance;

		OnCameraStateChanged?.Invoke(CameraStates.TOPDOWN);

		InputManager.Instance.OnMapViewEnterPressed -= SwitchToMap;
		InputManager.Instance.OnMapViewExitPressed += SwitchToPlayer;
		InputManager.Instance.DisableFPSMaps();
		InputManager.Instance.EnableMapViewMaps();

		foreach (ControllableEntity teammate in EntityManager.Instance.playerTeammates)
		{
			
			teammate.aiDetection.visionCone.SetActive(true);
		}

		currentState = CameraStates.TOPDOWN;
	}

	private void SwitchToPlayer()
	{
		//Change position
		Camera camera = Camera.main;
		camera.transform.position = playerCameraPos.position;

		//Return to previous rotation
		//camera.transform.rotation = prevCameraAngle;

		isMapView = false;

		//Lock cursor again
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		camera.orthographic = false;
		


		//Toggle movement
		movementController.canMove = true;
		cameraController.canLookAround = true;

		OnCameraStateChanged?.Invoke(CameraStates.FPS);

		InputManager.Instance.OnMapViewEnterPressed += SwitchToMap;
		InputManager.Instance.OnMapViewExitPressed -= SwitchToPlayer;
		InputManager.Instance.EnableFPSMaps();
		InputManager.Instance.DisableMapViewMaps();

		foreach (ControllableEntity teammate in EntityManager.Instance.playerTeammates)
		{
			teammate.aiDetection.visionCone.SetActive(false);
		}

		currentState = CameraStates.FPS;
	}

	private void OnDestroy()
	{
		
	}

	public void SwitchTarget(ControllableEntity newPlayer)
	{
		movementController = newPlayer.GetComponent<PlayerMovementController>();
		topDownCameraPos = newPlayer.GetComponentInChildren<CameraPos>().gameObject.transform;
		playerCameraPos = newPlayer.cameraPos;
	}
}
