using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStateSwitcher : MonoBehaviour
{
    public GameObject playerCameraPos;
	public Vector3 currentPos;
	public Vector3 targetPos;
	public Quaternion prevCameraAngle;
    public GameObject topDownCameraPos;
	public Camera camera;

	public PlayerMovementController movementController;
	public CameraController cameraController;

	public static event Action<CameraStates> OnCameraStateChanged;

	private void Start()
	{
		InputManager.Instance.OnMapViewEnterPressed += SwitchToMap;
		
	}

	private void SwitchToMap()
	{
		//Toggle movement
		movementController.canMove = false;
		cameraController.canLookAround = false;

		
		
		//Change position
		Camera.main.transform.position = topDownCameraPos.transform.position;

		//Cache old camera angle and rotate it downwards
		prevCameraAngle = Camera.main.transform.rotation;
		Quaternion newRotation = Quaternion.Euler(90f, prevCameraAngle.y, prevCameraAngle.z);
		Camera.main.transform.localRotation = newRotation;

		//Confine cursor to window
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		camera.orthographic = true;
		camera.orthographicSize = 23f;

		OnCameraStateChanged?.Invoke(CameraStates.TOPDOWN);

		InputManager.Instance.OnMapViewEnterPressed -= SwitchToMap;
		InputManager.Instance.OnMapViewExitPressed += SwitchToPlayer;
		InputManager.Instance.DisableFPSMaps();
		InputManager.Instance.EnableMapViewMaps();
	}

	private void SwitchToPlayer()
	{
		//Change position
		Camera camera = Camera.main;
		camera.transform.position = playerCameraPos.transform.position;

		//Return to previous rotation
		camera.transform.rotation = prevCameraAngle;

		

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
	}

	private void OnDestroy()
	{
		InputManager.Instance.OnMapViewExitPressed -= SwitchToPlayer;
		InputManager.Instance.OnMapViewEnterPressed -= SwitchToMap;
	}
}
