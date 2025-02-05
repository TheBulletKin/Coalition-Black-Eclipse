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

	//Command Creation
	public event Action<CommandType, bool> OnCommandCreatePressed;	
	//Teammate and Group selection
	public event Action<int> OnTeammateSelectPressed;
	public event Action<int> OnAiGroupSelectedPressed;
	//Go codes
	public event Action<int> OnGoCodePressed;
	//Movement
	public event Action OnSprintPressed;
	public event Action OnSprintReleased;
	public event Action OnJumpPressed;
	public event Action OnJumpReleased;
	public event Action OnCrouchPressed;
	public event Action OnCrouchReleased;
	//Map toggles
	public event Action OnMapViewEnterPressed;
	public event Action OnMapViewExitPressed;
	//Interaction
	public event Action OnInteractPressed;
	public event Action OnFirePressed;
	public event Action OnReloadPressed;
	

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
		controls.Enable();
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
		UnsubscribeFPSInputMaps();
		controls.FPS.Disable();
		
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

		//Command Creation
		controls.FPS.InstantCommand.performed += InstantCommandPerformed;
		controls.FPS.QueueCommand.performed += QueueCommandPerformed;

		//Teammate and Group selection
		controls.FPS.SelectAiTeammate.performed += SelectAiTeammatePerformed;
		controls.FPS.SelectAiGroup.performed += SelectAiGroupPerformed;
		//Go codes
		controls.FPS.ExecuteGoCode.performed += ActivateGoCodePerformed;
		//Movement
		controls.FPS.Sprint.started += SprintStarted;
		controls.FPS.Sprint.canceled += SprintCancelled;
		controls.FPS.Jump.started += JumpStarted;
		controls.FPS.Jump.canceled += JumpCancelled;
		controls.FPS.Crouch.started += CrouchStarted;
		controls.FPS.Crouch.canceled += CrouchCancelled;

		//Map toggles
		controls.FPS.EnterMapView.performed += EnterMapViewPerformed;

		//Interaction
		controls.FPS.Interact.performed += InteractPerformed;

		//Firing
		controls.FPS.Fire.performed += FirePerformed;

		//Reload
		controls.FPS.Reload.performed += ReloadPerformed;

	}

	public void UnsubscribeFPSInputMaps()
	{
		//Command Creation
		controls.FPS.InstantCommand.performed -= InstantCommandPerformed;
		controls.FPS.QueueCommand.performed -= QueueCommandPerformed;

		//Teammate and Group selection
		controls.FPS.SelectAiTeammate.performed -= SelectAiTeammatePerformed;
		controls.FPS.SelectAiGroup.performed -= SelectAiGroupPerformed;
		//Go codes
		controls.FPS.ExecuteGoCode.performed -= ActivateGoCodePerformed;
		//Movement
		controls.FPS.Sprint.started -= SprintStarted;
		controls.FPS.Sprint.canceled -= SprintCancelled;
		controls.FPS.Jump.started -= JumpStarted;
		controls.FPS.Jump.canceled -= JumpCancelled;
		controls.FPS.Crouch.started -= CrouchStarted;
		controls.FPS.Crouch.canceled -= CrouchCancelled;

		//Map toggles
		controls.FPS.EnterMapView.performed -= EnterMapViewPerformed;

		//Interaction
		controls.FPS.Interact.performed -= InteractPerformed;

		//Firing
		controls.FPS.Fire.performed -= FirePerformed;

		//Reload
		controls.FPS.Reload.performed -= ReloadPerformed;
	}

	public void SubscribeTopDownInputMaps()
	{
		//Map change key pressed
		controls.MapView.ExitMap.performed += OnMapViewExitPerformed;		
		
		//Command Creation
		controls.MapView.InstantCommand.performed += InstantCommandPerformed;
		controls.MapView.QueueCommand.performed += QueueCommandPerformed;

		//Teammate and Group selection
		controls.MapView.SelectAiTeammate.performed += SelectAiTeammatePerformed;
		controls.MapView.SelectAiGroup.performed += SelectAiGroupPerformed;
		//Go codes
		controls.MapView.ExecuteGoCode.performed += ActivateGoCodePerformed;
		
	}


	public void UnsubscribeTopDownInputMaps()
	{
		//Map change key pressed
		controls.MapView.ExitMap.performed -= OnMapViewExitPerformed;

		//Command Creation
		controls.MapView.InstantCommand.performed -= InstantCommandPerformed;
		controls.MapView.QueueCommand.performed -= QueueCommandPerformed;

		//Teammate and Group selection
		controls.MapView.SelectAiTeammate.performed -= SelectAiTeammatePerformed;
		controls.MapView.SelectAiGroup.performed -= SelectAiGroupPerformed;
		//Go codes
		controls.MapView.ExecuteGoCode.performed -= ActivateGoCodePerformed;
	}

	//----- Command creation
	private void InstantCommandPerformed(InputAction.CallbackContext context)
	{
		OnCommandCreatePressed?.Invoke(ParseCommandType(context), false);		
	}

	private void QueueCommandPerformed(InputAction.CallbackContext context)
	{
		OnCommandCreatePressed?.Invoke(ParseCommandType(context), true);		
	}

	private CommandType ParseCommandType(InputAction.CallbackContext context)
	{
		switch (context.control.name)
		{
			case "b":
				return CommandType.MOVE;		
			case "v":
				return CommandType.LOOK;				
			default:
				return CommandType.NONE;				
		}
	}

	//---- Teammate & Group selection
	private void SelectAiGroupPerformed(InputAction.CallbackContext context)
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

	private void SelectAiTeammatePerformed(InputAction.CallbackContext context)
	{
		switch (context.control.name)
		{
			case "backquote":
				OnTeammateSelectPressed?.Invoke(-1);
				break;
			case "f1":
				OnTeammateSelectPressed?.Invoke(0);
				break;
			case "f2":
				OnTeammateSelectPressed?.Invoke(1);
				break;
			case "f3":
				OnTeammateSelectPressed?.Invoke(2);
				break;
			case "f4":
				OnTeammateSelectPressed?.Invoke(3);
				break;
			default:
				break;
		}

	}

	//---- Go Codes
	private void ActivateGoCodePerformed(InputAction.CallbackContext context)
	{

		switch (context.control.name)
		{
			case "1":
				OnGoCodePressed?.Invoke(1);
				break;			
			default:
				break;
		}
	}


	//---- Movement
	private void SprintStarted(InputAction.CallbackContext context)
	{
		OnSprintPressed?.Invoke();
	}

	private void SprintCancelled(InputAction.CallbackContext context)
	{
		OnSprintReleased?.Invoke();
	}
	private void JumpStarted(InputAction.CallbackContext context)
	{
		OnJumpPressed?.Invoke();
	}

	private void JumpCancelled(InputAction.CallbackContext context)
	{
		OnJumpReleased?.Invoke();
	}

	private void CrouchStarted(InputAction.CallbackContext context)
	{
		OnCrouchPressed?.Invoke();
	}

	private void CrouchCancelled(InputAction.CallbackContext context)
	{
		OnCrouchReleased?.Invoke();
	}

	//---- Map Toggles
	private void EnterMapViewPerformed(InputAction.CallbackContext context)
	{
		OnMapViewEnterPressed?.Invoke();
	}

	private void OnMapViewExitPerformed(InputAction.CallbackContext context)
	{
		OnMapViewExitPressed?.Invoke();
	}


	//---- Interaction
	private void InteractPerformed(InputAction.CallbackContext context)
	{
		OnInteractPressed?.Invoke();
	}

	//---- Firing
	private void FirePerformed(InputAction.CallbackContext context)
	{
		OnFirePressed?.Invoke();
	}

	//---- Reloading
	private void ReloadPerformed(InputAction.CallbackContext context)
	{
		OnReloadPressed?.Invoke();
	}


	void Update()
	{
		//Look and move happen often enough where they should stay as continuously polling
		lookAxis = controls.FPS.MouseAim.ReadValue<Vector2>();
		moveAxis = controls.FPS.Move.ReadValue<Vector2>();
	}
}

