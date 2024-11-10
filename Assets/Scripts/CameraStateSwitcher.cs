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

		InputManager.Instance.OnMapViewEnterPressed -= SwitchToMap;
		InputManager.Instance.OnMapViewExitPressed += SwitchToPlayer;
		InputManager.Instance.DisableFPSMaps();
		InputManager.Instance.EnableMapViewMaps();
		
		//Change position
		Camera.main.transform.position = topDownCameraPos.transform.position;

		//Cache old camera angle and rotate it downwards
		prevCameraAngle = Camera.main.transform.rotation;
		Quaternion newRotation = Quaternion.Euler(90f, prevCameraAngle.y, prevCameraAngle.z);
		Camera.main.transform.localRotation = newRotation;

		//Confine cursor to window
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		OnCameraStateChanged?.Invoke(CameraStates.TOPDOWN);
	}

	private void SwitchToPlayer()
	{
		//Change position
		Camera.main.transform.position = playerCameraPos.transform.position;

		//Return to previous rotation
		Camera.main.transform.rotation = prevCameraAngle;

		InputManager.Instance.OnMapViewEnterPressed += SwitchToMap;
		InputManager.Instance.OnMapViewExitPressed -= SwitchToPlayer;
		InputManager.Instance.EnableFPSMaps();
		InputManager.Instance.DisableMapViewMaps();

		//Lock cursor again
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		//Toggle movement
		movementController.canMove = true;
		cameraController.canLookAround = true;

		OnCameraStateChanged?.Invoke(CameraStates.FPS);
	}

	private void OnDestroy()
	{
		InputManager.Instance.OnMapViewExitPressed -= SwitchToPlayer;
		InputManager.Instance.OnMapViewEnterPressed -= SwitchToMap;
	}
}
