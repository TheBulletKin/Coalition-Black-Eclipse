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
	public event Action OnMapViewEnterPressed;
	public event Action OnMapViewExitPressed;
	public event Action OnCommandCreatePressed;
	public event Action OnQueueCommandPressed;
	public event Action<int> OnTeammateSelectPressed;	
	public event Action<int> OnAiGroupSelectedPressed;
	public event Action OnGoCodePressed;

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

		//Map change key pressed
		controls.FPS.EnterMapView.performed += OnMapViewEnterPerformed;

		//Queue command key pressed
		controls.FPS.QueueCommand.performed += OnQueueCommandPerformed;

		//Selecting teammates to assign commands with F1, F2 etc.
		controls.FPS.SelectAiTeammate.performed += SelectAiTeammate;

		//Activting currently selected team / groups commands
		controls.FPS.ExecuteGoCode.performed += ActivateGoCode;		

		controls.FPS.SelectAiGroup.performed += SelectAiGroup;
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

		//Map change key pressed
		controls.FPS.EnterMapView.performed -= OnMapViewEnterPerformed;

		//Queue command key pressed
		controls.FPS.QueueCommand.performed -= OnQueueCommandPerformed;

		//Selecting teammates to assign commands with F1, F2 etc.
		controls.FPS.SelectAiTeammate.performed -= SelectAiTeammate;

		//Executing individual commands with `,1,2,3 etc
		controls.FPS.ExecuteGoCode.performed += ActivateGoCode;
		

		controls.FPS.SelectAiGroup.performed -= SelectAiGroup;
	}

	


	public void SubscribeTopDownInputMaps()
	{
		//Map change key pressed
		controls.MapView.ExitMap.performed += OnMapViewExitPerformed;

		//Create command key pressed on map
		controls.MapView.CreateCommand.performed += OnCommandCreatePerformed;
	}

	

	public void UnsubscribeTopDownInputMaps()
	{
		//Map change key pressed
		controls.MapView.ExitMap.performed -= OnMapViewExitPerformed;

		//Create command key pressed on map
		controls.MapView.CreateCommand.performed -= OnCommandCreatePerformed;
	}

	private void SelectAiGroup(InputAction.CallbackContext context)
	{
		
		switch (context.control.name)
		{
			case "backquote":
				
				OnAiGroupSelectedPressed?.Invoke(-1);
				break;
			case "f1":
				
				OnAiGroupSelectedPressed?.Invoke(0);
				break;
			case "f2":
			
				OnAiGroupSelectedPressed?.Invoke(1);
				break;
			case "f3":
				
				OnAiGroupSelectedPressed?.Invoke(2);
				break;
			default:
				break;
		}
	}

	private void ActivateGoCode(InputAction.CallbackContext context)
	{
		//Eventually change to tackle multiple go codes
		OnGoCodePressed?.Invoke();
	}	

	private void SelectAiTeammate(InputAction.CallbackContext context)
	{
		switch (context.control.name.Substring(1, 1))
		{
			case "1":
				OnTeammateSelectPressed?.Invoke(0);
				break;
			case "2":
				OnTeammateSelectPressed?.Invoke(1);
				break;
			case "3":
				OnTeammateSelectPressed?.Invoke(2);
				break;
			case "4":
				OnTeammateSelectPressed?.Invoke(3);
				break;
			default:
				break;
		}
		
	}
	private void OnQueueCommandPerformed(InputAction.CallbackContext context)
	{
		OnQueueCommandPressed?.Invoke();
	}

	private void OnCommandCreatePerformed(InputAction.CallbackContext context)
	{
		OnCommandCreatePressed?.Invoke();
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

