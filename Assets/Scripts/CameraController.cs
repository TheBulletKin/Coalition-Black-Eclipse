using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	//Base camera variables
	[SerializeField] public bool canLookAround = true;
	[SerializeField] private float lookSensitivity = 1f;
	[SerializeField] private float lookSensModifier = 1.0f;

	//Camera rotation values
	[SerializeField] private float cameraPitch;
	[SerializeField] private float cameraPitchMax = 90f;
	[SerializeField] private float cameraPitchMin = -90f;
	[SerializeField] private float cameraYaw;
	[SerializeField] private float cameraYawMax = 360f;
	[SerializeField] private float cameraYawMin = 0f;
	[SerializeField] private float cameraRoll;

	//Camera position values
	[SerializeField] private Vector3 targetPosition;
	[SerializeField] private Vector3 currentPosition;

	public Vector3 playerStartLocalRotation;
	public Vector3 cameraStartLocalPosition;

	//Attached player
	public Transform playerTr;
	// Assign player entity in inspector
	[SerializeField] private PlayerEntity player;

	private void Start()
	{
		playerStartLocalRotation = Vector3.zero;
		cameraStartLocalPosition = transform.localPosition;
	}

	private void Update()
	{

		float deltaTime = Time.deltaTime;
		Vector2 inputVector = Vector2.zero;

		//Will eventually be controlled by the control preset config - fps / loading etc.
		if (canLookAround)
		{
			inputVector = InputManager.Instance.LookAxis;
			inputVector = Vector2.Scale(inputVector, new Vector2(lookSensitivity * lookSensModifier, lookSensitivity * lookSensModifier));
			SetCameraYaw(cameraYaw + inputVector.x);

			SetCameraPitch(cameraPitch - inputVector.y);

			//Yaw should rotate the player's entire gameObject on the Y axis, so split the two into different vectors for easier calculation
			Vector3 pitchVector = new Vector3(cameraPitch, 0f, 0f);
			Vector3 yawVector = new Vector3(0f, cameraYaw, 0f);

			//Get the yaw only and rotate the player object
			playerTr.localEulerAngles = yawVector;
			transform.localEulerAngles = pitchVector;

		}


	}


	//Both camera pitch and yaw changes the variables and clamps but doesn't rotate yet

	private void SetCameraPitch(float pitch)
	{
		cameraPitch = pitch;
		if (cameraPitch < cameraPitchMin)
		{
			cameraPitch = cameraPitchMin;
		}
		else if (cameraPitch > cameraPitchMax)
		{
			cameraPitch = cameraPitchMax;
		}
	}

	private void SetCameraYaw(float yaw)
	{
		cameraYaw = yaw;
		cameraYaw = yaw - Mathf.Floor(yaw / 360) * 360;

	}

	public void TransferOwnership(GameObject newOwner)
	{
		playerTr = newOwner.transform;
	}

	public void AimInDirection(Vector3 direction)
	{
		direction.y = 0f;
		
		if (direction == Vector3.zero)
		{
			return;
		}
		direction.Normalize();

		Vector3 flatDir = new Vector3(direction.x, 0f, direction.z);
		if (flatDir.sqrMagnitude > 0.001f)
		{
			cameraYaw = Quaternion.LookRotation(flatDir).eulerAngles.y;
			if (playerTr != null)
			{
				playerTr.localEulerAngles = new Vector3(0f, cameraYaw, 0f);
			}
		}

		float pitchAngle = Vector3.SignedAngle(flatDir, direction, Vector3.right); // rotate around X-axis
		cameraPitch = Mathf.Clamp(pitchAngle, cameraPitchMin, cameraPitchMax);

		transform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
	}


}
