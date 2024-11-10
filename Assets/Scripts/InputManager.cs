using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
	/* In order to access user inputs, read from this class.
	 * Add button states here and have this as a level of abstraction, using the new input system.
	 */
	

	private BaseControls controls;

	//Imeplements singletone pattern
	private static InputManager instance;
	public static InputManager Instance
	{
		get
		{
			//This will double check that the instance is actually active if it is called before it enables itself
			if (instance == null)
			{
				instance = FindObjectOfType<InputManager>();

				if (instance == null)
				{
					Debug.LogError("InputManager instance not found in the scene");
				}
			}
			return instance;
		}
	}

	//Private
	private Vector2 lookAxis;
	private static Vector2 moveAxis;

	//Public accessors
	/// <summary>
	/// A vector 2 describing the mouse movement delta
	/// </summary>
	public Vector2 LookAxis => lookAxis;
	/// <summary>
	/// A vector 2 describing the movement keys pressed (WASD)
	/// </summary>
	public Vector2 MoveAxis => moveAxis;

	public event Action OnSprintPressed;
	public event Action OnSprintReleased;
	public event Action OnJumpPressed;
	public event Action OnJumpReleased;
	public event Action OnCrouchPressed;
	public event Action OnCrouchReleased;
	public event Action OnInteractPressed;
	public event Action OnExecutePressed;
	public event Action OnMapViewEnterPressed;
	public event Action OnMapViewExitPressed;
	public event Action OnCommandCreatePressed;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
		controls = new BaseControls();
	}

	private void OnEnable()
	{
		controls.FPS.Enable();
		SubscribeFPSInputMaps();

	}	

	private void OnDestroy()
	{

		UnsubscribeFPSInputMaps();
		UnsubscribeTopDownInputMaps();

		controls.Disable();
	}

	public void DisableFPSMaps()
	{
		controls.FPS.Disable();
		UnsubscribeFPSInputMaps();
	}

	public void DisableMapViewMaps()
	{
		controls.MapView.Disable();
		UnsubscribeTopDownInputMaps();
	}

	public void EnableFPSMaps()
	{
		controls.FPS.Enable();
		SubscribeFPSInputMaps();
	}

	public void EnableMapViewMaps()
	{
		controls.MapView.Enable();
		SubscribeTopDownInputMaps();
	}

	public void SubscribeFPSInputMaps()
	{
		//Sprint started and cancelled
		controls.FPS.Sprint.started += OnSprintStarted;
		controls.FPS.Sprint.canceled += OnSprintCanceled;

		//Jump started and cancelled
		controls.FPS.Jump.started += OnJumpStarted;
		controls.FPS.Jump.canceled += OnJumpCanceled;

		//Crouch started and cancelled
		controls.FPS.Crouch.started += OnCrouchStarted;
		controls.FPS.Crouch.canceled += OnCrouchCanceled;

		//Interact pressed
		controls.FPS.Interact.performed += OnInteractPerformed;

		//Execute command pressed
		controls.FPS.ExecuteCommand.performed += OnExecutePerformed;

		//Map change key pressed
		controls.FPS.EnterMapView.performed += OnMapViewEnterPerformed;
	}

	public void UnsubscribeFPSInputMaps()
	{
		//Sprint started and cancelled
		controls.FPS.Sprint.started -= OnSprintStarted;
		controls.FPS.Sprint.canceled -= OnSprintCanceled;

		//Jump started and cancelled
		controls.FPS.Jump.started -= OnJumpStarted;
		controls.FPS.Jump.canceled -= OnJumpCanceled;

		//Crouch started and cancelled
		controls.FPS.Crouch.started -= OnCrouchStarted;
		controls.FPS.Crouch.canceled -= OnCrouchCanceled;

		//Interact pressed
		controls.FPS.Interact.performed -= OnInteractPerformed;

		//Execute command pressed
		controls.FPS.ExecuteCommand.performed -= OnExecutePerformed;

		//Map change key pressed
		controls.FPS.EnterMapView.performed -= OnMapViewEnterPerformed;
	}

	

	public void SubscribeTopDownInputMaps()
	{
		//Map change key pressed
		controls.MapView.ExitMap.performed += OnMapViewExitPerformed;

		//Create command key pressed on map
		controls.MapView.CreateCommand.performed += OnCommandCreatePerformed;

		//On execute command pressed
		controls.MapView.ExecuteCommand.performed += OnExecutePerformed;
	}

	

	public void UnsubscribeTopDownInputMaps()
	{
		//Map change key pressed
		controls.MapView.ExitMap.performed -= OnMapViewExitPerformed;

		//Create command key pressed on map
		controls.MapView.CreateCommand.performed -= OnCommandCreatePerformed;

		//On execute command pressed
		controls.MapView.ExecuteCommand.performed -= OnExecutePerformed;
	}

	private void OnCommandCreatePerformed(InputAction.CallbackContext context)
	{
		OnCommandCreatePressed?.Invoke();
	}

	private void OnExecutePerformed(InputAction.CallbackContext context)
	{
		OnExecutePressed?.Invoke();
	}

	private void OnSprintStarted(InputAction.CallbackContext context)
	{
		OnSprintPressed?.Invoke();
	}

	private void OnSprintCanceled(InputAction.CallbackContext context)
	{
		OnSprintReleased?.Invoke();
	}

	private void OnMapViewEnterPerformed(InputAction.CallbackContext context)
	{
		OnMapViewEnterPressed?.Invoke();
	}

	private void OnMapViewExitPerformed(InputAction.CallbackContext context)
	{
		OnMapViewExitPressed?.Invoke();
	}

	private void OnJumpStarted(InputAction.CallbackContext context)
	{
		OnJumpPressed?.Invoke();
	}

	private void OnJumpCanceled(InputAction.CallbackContext context)
	{
		OnJumpReleased?.Invoke();
	}

	private void OnCrouchStarted(InputAction.CallbackContext context)
	{
		OnCrouchPressed?.Invoke();
	}

	private void OnCrouchCanceled(InputAction.CallbackContext context)
	{
		OnCrouchReleased?.Invoke();
	}

	private void OnInteractPerformed(InputAction.CallbackContext context)
	{
		OnInteractPressed?.Invoke();
	}


	void Update()
	{
		//Look and move happen often enough where they should stay as continuously polling
		lookAxis = controls.FPS.MouseAim.ReadValue<Vector2>();
		moveAxis = controls.FPS.Move.ReadValue<Vector2>();
	}
}

