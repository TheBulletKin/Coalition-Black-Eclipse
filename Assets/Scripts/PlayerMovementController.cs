using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerMovementController : MonoBehaviour
{
	public bool canMove = true;
	private bool ShouldSprint => isSprinting && movespeedSettings.canSprint && characterController.isGrounded;
	private bool ShouldJump => isJumping && characterController.isGrounded;
	private bool ShouldCrouch => isCrouching && !duringCrouchAnimation && characterController.isGrounded;


	public bool isGrounded { get; private set; }
	public bool hasJumpedThisFrame { get; private set; }
	[SerializeField] private bool isSprinting = false;
	[SerializeField] private bool isCrouched = false;
	[SerializeField] private bool isJumping = false;


	[Serializable]
	public class MovespeedSettings
	{
		public bool canSprint = true;
		public float walkSpeed = 3.0f;
		public float sprintSpeed = 3.0f;
		public float crouchSpeed = 0.5f;
		public float slopeSpeed = 8.0f;
		public float movementInertia = 15f;
		public float airAcceleration = 25f;
		public float maxAirSpeed = 10f;
	}

	public MovespeedSettings movespeedSettings;


	[Serializable]
	public class JumpSettings
	{
		public bool canJump = true;
		public float jumpForce = 8.0f;
		public float gravity = 10.0F;

	}

	public JumpSettings jumpSettings;
	private float lastTimeJumped = 0f;

	[Serializable]
	public class CrouchSettings
	{
		public bool canCrouch = true;
		public float crouchHeight = 0.5f;
		public float standingHeight = 2f;
		public float timeToCrouch = 0.5f;
		public Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
		public Vector3 standingCenter = new Vector3(0, 0, 0);
		public float crouchSpeed = 0.5f;
	}

	public CrouchSettings crouchSettings;

	[Header("Crouch States")]
	private bool isCrouching;
	private bool duringCrouchAnimation;
	private float targetCharacterHeight;

	//Settings for what layers are considered the ground
	[Serializable]
	public class GroundedCheckSettings
	{
		public RaycastHit groundHit;
		public bool isNearGrounded;
		public LayerMask groundLayer;
		public float groundCheckDistance = 0.05f;
		public float groundCheckDistanceInAir = 0.07f;
		public float jumpGroundingPreventionTime = 0.2f;
	}
	public GroundedCheckSettings groundedCheckSettings;

	Vector3 groundNormal;


	[Header("Additional Values")]
	private Camera playerCamera;
	private CameraController cameraController;
	private CharacterController characterController;

	//Movement values
	private Vector3 moveDirection;
	private Vector2 currentMoveInput;
	private Vector3 lastVelocity;
	/// <summary>
	/// The final velocity considering moveDirection, speed and deltaTime
	/// </summary>
	private Vector3 moveVelocity;
	public Vector3 characterVelocity;
	private Vector3 latestImpactSpeed;
	
	public float CameraHeightRatio = 0.9f; //0-1 Value, 1 is top of character model, 0 is feet.

	//Rotation values
	private float rotationX = 0;
	private float rotationY = 0;


	private void Awake()
	{
		characterController = GetComponent<CharacterController>();
		cameraController = GetComponentInChildren<CameraController>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		playerCamera = GetComponentInChildren<Camera>();
	}

	private void Start()
	{
		InputManager.Instance.OnCrouchPressed += HandleCrouch;
		InputManager.Instance.OnSprintPressed += HandleSprint;
		InputManager.Instance.OnSprintReleased += HandleSprintReleased;
		InputManager.Instance.OnJumpPressed += HandleJumpPressed;
		InputManager.Instance.OnJumpReleased += HandleJumpReleased;
	}

	private void OnDisable()
	{
		if (InputManager.Instance != null)
		{
			InputManager.Instance.OnCrouchPressed -= HandleCrouch;
			InputManager.Instance.OnSprintPressed -= HandleSprint;
			InputManager.Instance.OnSprintReleased -= HandleSprintReleased;
			InputManager.Instance.OnJumpPressed -= HandleJumpPressed;
			InputManager.Instance.OnJumpReleased -= HandleJumpReleased;
		}

	}

	private void Update()
	{


		if (canMove)
		{
			bool wasGrounded = isGrounded;
			CheckGrounded();

            if (ShouldCrouch)
            {
                
            }

            CalculateVelocities();

			ApplyFinalMovements();

		}
	}

	private void CalculateVelocities()
	{
		currentMoveInput = InputManager.Instance.MoveAxis;

		float currentSpeed;


		if (ShouldSprint)
		{
			currentSpeed = movespeedSettings.sprintSpeed;
		}
		else if (ShouldCrouch)
		{
			currentSpeed = movespeedSettings.crouchSpeed;
			StartCoroutine(CrouchStand());
		}
		else
		{
			currentSpeed = movespeedSettings.walkSpeed;
		}

		if (ShouldJump)
		{
			moveDirection.y = jumpSettings.jumpForce;
		}	

		//moveDirection is a world space vector, so the local vector is transformed into world space
		moveDirection = ((transform.TransformDirection(Vector3.forward) * currentMoveInput.y)
			+ (transform.TransformDirection(Vector3.right) * currentMoveInput.x))
			* currentSpeed;	


		if (isGrounded)
		{
			Vector3 targetVelocity = moveDirection;
            if (isCrouching)
            {
				targetVelocity = targetVelocity.normalized;
				targetVelocity *= movespeedSettings.crouchSpeed;
            }

			characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity,	movespeedSettings.movementInertia * Time.deltaTime);

			if (isJumping)
			{
				characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);
				characterVelocity += Vector3.up * jumpSettings.jumpForce;
				lastTimeJumped = Time.time;
				isGrounded = false;
				groundNormal = Vector3.up;
			}
		}
		else
		{
			characterVelocity += moveDirection * movespeedSettings.airAcceleration * Time.deltaTime;

			float verticalVelocity = characterVelocity.y;
			Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterVelocity, Vector3.up);
			horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, movespeedSettings.maxAirSpeed);
			characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);
			
			characterVelocity += Vector3.down * jumpSettings.gravity * Time.deltaTime;
		}

	}

	private IEnumerator CrouchStand()
	{
		if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 2f))
			yield break;

		duringCrouchAnimation = true;

		float timeElapsed = 0;
		float targetHeight = isCrouching ? crouchSettings.standingHeight : crouchSettings.crouchHeight;
		float currentHeight = characterController.height;
		Vector3 targetCenter = isCrouching ? crouchSettings.standingCenter : crouchSettings.crouchingCenter;
		Vector3 currentCenter = characterController.center;

		while (timeElapsed < crouchSettings.timeToCrouch)
		{
			characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / crouchSettings.timeToCrouch);
			characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / crouchSettings.timeToCrouch);
			timeElapsed += Time.deltaTime;
			yield return null;
		}

		characterController.height = targetHeight;
		characterController.center = targetCenter;

		isCrouching = !isCrouching;

		duringCrouchAnimation = false;
	}

	private void SetCrouchStateHeight()
	{
		if (isCrouching)
		{
			targetCharacterHeight = crouchSettings.crouchHeight;
		}
		else
		{
			targetCharacterHeight = crouchSettings.standingHeight;
		}
	}

	private void UpdateCharacterHeight()
	{
		/* For instance height change
		m_Controller.height = m_TargetCharacterHeight;
		m_Controller.center = Vector3.up * m_Controller.height * 0.5f;
		PlayerCamera.transform.localPosition = Vector3.up * m_TargetCharacterHeight * CameraHeightRatio;
		m_Actor.AimPoint.transform.localPosition = m_Controller.center;
		*/

		// resize the capsule and adjust camera position
		characterController.height = Mathf.Lerp(characterController.height, targetCharacterHeight, crouchSettings.crouchSpeed * Time.deltaTime);
		characterController.center = Vector3.up * characterController.height * 0.5f;
		playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, Vector3.up * targetCharacterHeight * CameraHeightRatio, crouchSettings.crouchSpeed * Time.deltaTime);
		//m_Actor.AimPoint.transform.localPosition = characterController.center;
	}

	private void ApplyFinalMovements()
	{
		Vector3 capsuleBottomBeforeMove = transform.position + (transform.up * characterController.radius);
		Vector3 capsuleTopBeforeMove = transform.position + (transform.up * (characterController.height - characterController.radius));
		characterController.Move(characterVelocity * Time.deltaTime);

		// detect obstructions to adjust velocity accordingly
		latestImpactSpeed = Vector3.zero;
		if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, characterController.radius, characterVelocity.normalized, out RaycastHit hit, characterVelocity.magnitude * Time.deltaTime, -1, QueryTriggerInteraction.Ignore))
		{
			// We remember the last impact speed because the fall damage logic might need it
			latestImpactSpeed = characterVelocity;

			characterVelocity = Vector3.ProjectOnPlane(characterVelocity, hit.normal);
		}


		/*
		lastVelocity = moveVelocity;
		if (!characterController.isGrounded)
		{
			moveDirection.y -= jumpSettings.gravity * Time.deltaTime;
		}


		moveVelocity = moveDirection * Time.deltaTime;
		characterController.Move(moveVelocity);
		*/


	}

	//Raycast from character centre, change to true if successful
	private void CheckGrounded()

	{
		float chosenGroundCheckDistance = isGrounded ? (characterController.skinWidth + groundedCheckSettings.groundCheckDistanceInAir) : groundedCheckSettings.groundCheckDistance;
		isGrounded = false;
		groundNormal = Vector3.up;

		if (Time.time >= lastTimeJumped + groundedCheckSettings.jumpGroundingPreventionTime)
		{
			if (Physics.CapsuleCast(transform.position + (transform.up * characterController.radius), transform.position + (transform.up * (characterController.height - characterController.radius)), characterController.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundedCheckSettings.groundLayer, QueryTriggerInteraction.Ignore))
			{
				groundNormal = hit.normal;

				isGrounded = true;

				// handle snapping to the ground
				if (hit.distance > characterController.skinWidth)
				{
					characterController.Move(Vector3.down * hit.distance);
				}
			}
		}

	}

	private void HandleCrouch()
	{
		isCrouching = true;
	}

	private void HandleCrouchRelease()
	{
		isCrouching = false;
	}

	private void HandleSprint()
	{
		isSprinting = true;
	}

	private void HandleSprintReleased()
	{
		isSprinting = false;
	}

	private void HandleJumpPressed()
	{
		if (isGrounded)
		{		
			isJumping = true;	

		}


	}

	private void HandleJumpReleased()
	{
		isJumping = false;
	}


}
