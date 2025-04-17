using System;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour, IToggleable
{
	public bool canMove = true;
	//Properties - To avoid multiple if statements later. May change if need be.
	private bool ShouldSprint => isSprinting && movespeedSettings.canSprint && isGrounded;
	private bool ShouldJump => isJumping && isGrounded;
	private bool ShouldCrouch => isCrouching && isGrounded;

	[Header("Movement States")]
	[SerializeField] private bool isGrounded;
	private bool hasJumpedThisFrame;
	[SerializeField] private bool isSprinting = false;
	[SerializeField] private bool isCrouching = false;
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
	[Header("Configurations")]
	public MovespeedSettings movespeedSettings;

	//Jump force and gravity settings
	[Serializable]
	public class JumpSettings
	{
		public bool canJump = true;
		public float jumpForce = 8.0f;
		public float gravity = 10.0F;

	}
	public JumpSettings jumpSettings;
	private float lastTimeJumped = 0f;

	//Settings for crouch height, speed, positions etc
	[Serializable]
	public class CrouchSettings
	{
		public bool canCrouch = true;
		public float crouchHeight = 0.8f;
		public float standingHeight = 2f;
		public float timeToCrouch = 0.5f;
		public Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
		public Vector3 standingCenter = new Vector3(0, 0, 0);
		public float crouchSpeed = 1.2f;
		public AnimationCurve crouchSpeedCurve;
		public float crouchTime = 0.7f;

	}
	public CrouchSettings crouchSettings;

	[Header("Crouch States")]
	private bool duringCrouchAnimation;
	public float targetCharacterHeight;
	//0-1 Value, 1 is top of character model, 0 is feet.
	public float cameraHeightRatio = 0.36f;
	public float crouchTimer = 0f;
	public bool crouchInProgress = false;
	private float originalCharacterHeight;
	public bool wasCrouching = false;


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
	private CharacterController characterController;
	private ShootingSystem shootingSystem;

	//Movement values
	private Vector3 moveDirection;
	private Vector2 currentMoveInput;
	private Vector3 lastVelocity;
	/// <summary>
	/// The final velocity considering moveDirection, speed and deltaTime
	/// </summary>
	public Vector3 characterVelocity;
	private Vector3 latestImpactSpeed;

	//Rotation values
	private float rotationX = 0;
	private float rotationY = 0;


	private void Awake()
	{
		characterController = GetComponent<CharacterController>();
		shootingSystem = GetComponent<ShootingSystem>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		playerCamera = Camera.main;
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	private void Start()
	{
		

		//UpdateCharacterHeight(true);
	}

	private void OnDestroy()
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


			if (crouchInProgress)
			{
				UpdateCharacterHeight(false);
			}


			CalculateVelocities();

			ApplyFinalMovements();

			if (shootingSystem != null)
			{
				shootingSystem.SpreadMultiplierFromVelocity(characterController.velocity, movespeedSettings.walkSpeed);
			}

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

			characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, movespeedSettings.movementInertia * Time.deltaTime);

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


	private void UpdateCharacterHeight(bool instant)
	{

		if (instant)
		{
			characterController.height = targetCharacterHeight;
			characterController.center = Vector3.up * characterController.height * 0.5f;
			playerCamera.transform.localPosition = Vector3.up * targetCharacterHeight * cameraHeightRatio;

		}
		else
		{
			//Time values to evaluate curve
			crouchTimer += Time.deltaTime;
			float normalizedTime = crouchTimer / crouchSettings.crouchTime;

			float curveFactor = crouchSettings.crouchSpeedCurve.Evaluate(normalizedTime);

			//User lerp to change controller height, camera position and centre using curve
			characterController.height = Mathf.Lerp(characterController.height, targetCharacterHeight, curveFactor);
			characterController.center = Vector3.up * characterController.height * 0.5f;

			playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, Vector3.up * targetCharacterHeight * cameraHeightRatio, curveFactor);

			//When the crouch timer reaches the full duration, set the values to their definite ones and reset the timer
			if (crouchTimer >= crouchSettings.crouchTime)
			{
				characterController.height = targetCharacterHeight;
				characterController.center = Vector3.up * characterController.height * 0.5f;
				playerCamera.transform.localPosition = Vector3.up * targetCharacterHeight * cameraHeightRatio;

				crouchInProgress = false;
				crouchTimer = 0;

			}
			else
			{
				crouchInProgress = true;
			}
		}
	}

	private void ApplyFinalMovements()
	{
		Vector3 capsuleBottomBeforeMove = transform.position + (transform.up * characterController.radius);
		Vector3 capsuleTopBeforeMove = transform.position + (transform.up * (characterController.height - characterController.radius));
		characterController.Move(characterVelocity * Time.deltaTime);

		latestImpactSpeed = Vector3.zero;
		if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, characterController.radius, characterVelocity.normalized, out RaycastHit hit, characterVelocity.magnitude * Time.deltaTime, -1, QueryTriggerInteraction.Ignore))
		{
			//Hold the last speed before a surface impact for fall damage later
			latestImpactSpeed = characterVelocity;

			characterVelocity = Vector3.ProjectOnPlane(characterVelocity, hit.normal);
		}


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
		if (isGrounded)
		{
			isCrouching = !isCrouching;

			crouchInProgress = true;

			if (isCrouching == true)
			{

				targetCharacterHeight = crouchSettings.crouchHeight;
			}
			else
			{
				targetCharacterHeight = crouchSettings.standingHeight;
			}
		}
		else
		{
			isCrouching = false;
			crouchInProgress = false;
		}

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

	public void DisableControl()
	{
		InputManager.Instance.OnCrouchPressed -= HandleCrouch;
		InputManager.Instance.OnSprintPressed -= HandleSprint;
		InputManager.Instance.OnSprintReleased -= HandleSprintReleased;
		InputManager.Instance.OnJumpPressed -= HandleJumpPressed;
		InputManager.Instance.OnJumpReleased -= HandleJumpReleased;

		enabled = false;
	}

	public void EnableControl()
	{
		InputManager.Instance.OnCrouchPressed += HandleCrouch;
		InputManager.Instance.OnSprintPressed += HandleSprint;
		InputManager.Instance.OnSprintReleased += HandleSprintReleased;
		InputManager.Instance.OnJumpPressed += HandleJumpPressed;
		InputManager.Instance.OnJumpReleased += HandleJumpReleased;

		enabled = true;
	}
}
