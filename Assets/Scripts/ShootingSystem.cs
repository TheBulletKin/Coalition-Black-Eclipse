using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class ShootingSystem : MonoBehaviour, IToggleable
{
	Camera mainCam;
	public Transform cameraPos;

	[SerializeField] LayerMask hittableLayers;

	[SerializeField] public WeaponConfig weaponConfig;
	[SerializeField] public int currentAmmo;
	[SerializeField] public int reserveAmmo;
	private float fireTimer = 0;
	private bool isReloading = false;
	private bool isFireRecovery = false;
	private bool inPlayerControl = false;
	private GameObject heldObject;
	private bool isHoldingObject;
	[SerializeField] private float heldObjectLaunchForce = 15f;
	[Tooltip("Angle of spread from the camera")]
	[Range(0f, 45f)]
	[SerializeField] public float baseSpreadAngle = 20f;
	[SerializeField] public float movementSpreadMultiplier = 1f;
	[SerializeField] private float movementMultiplierWeighting = 0.25f;
	[SerializeField] public float currentBaseSpread = 1.0f;
	[SerializeField] public float crosshairConvergeanceRange = 20.0f;
	[SerializeField] private AnimationCurve speedSpreadCurve;
	[SerializeField] private PlayerMovementController playerMovementController;
	[SerializeField] private Quaternion lastFrameRotation;
	private float rotationSpread;
	




	public event Action<int, int> WeaponFired;


	void Start()
	{
		mainCam = Camera.main;
		currentAmmo = weaponConfig ? weaponConfig.maxAmmo : 10;
		inPlayerControl = false;
		isHoldingObject = false;
		UpdateAmmo(currentAmmo, reserveAmmo);
		playerMovementController = GetComponent<PlayerMovementController>();
		lastFrameRotation = transform.rotation;
	}

	// Update is called once per frame
	void Update()
	{
		if (inPlayerControl)
		{
			Quaternion currentRotation = mainCam.transform.rotation;

			//Multiplying forward by the rotation creates a vector that points in that direction
			Vector3 currentForward = currentRotation * Vector3.forward;
			Vector3 lagForward = lastFrameRotation * Vector3.forward;

			float angle = Vector3.Angle(currentForward, lagForward);


			if (angle > weaponConfig.maxRotationAngle)
			{
				lagForward = Vector3.RotateTowards(currentForward, lagForward, weaponConfig.maxRotationAngle * Mathf.Deg2Rad, 0f);
				lastFrameRotation = Quaternion.LookRotation(lagForward);
			}
			


			float angularDifference = Quaternion.Angle(currentRotation, lastFrameRotation);

		

			//Value of 0-1 based on difference between forward vector, lag vector and max weapon angle
			float rotationSpreadValue = Mathf.Clamp01(angularDifference / weaponConfig.maxRotationAngle);

			//Apply a curve that can smoothly control the spread influence
			rotationSpreadValue = weaponConfig.rotationInfluenceCurve.Evaluate(rotationSpreadValue);

			//Apply a flat multiplier for rotational spread influence
			rotationSpreadValue = rotationSpreadValue * weaponConfig.rotationInfluence;

			//Add a final clamp to control maximum possible spread
			rotationSpreadValue = Mathf.Min(rotationSpreadValue, weaponConfig.rotationSpreadMax);

			//Raw rotational spread value is the base spread * this final multiplier
			float rawRotationSpread = baseSpreadAngle * rotationSpreadValue;

			
			


			float totalAngle = (baseSpreadAngle * movementSpreadMultiplier) + (baseSpreadAngle * (rotationSpreadValue * weaponConfig.rotationInfluence));
			float spreadRadians = totalAngle * Mathf.Deg2Rad;

			currentBaseSpread = Mathf.Tan(spreadRadians) * crosshairConvergeanceRange * 15f;

			lastFrameRotation = Quaternion.RotateTowards(lastFrameRotation, currentRotation, weaponConfig.rotationalSpreadDecreaseSpeed * Time.deltaTime);
		}
		else
		{
			currentBaseSpread = 0f;
		}

		Debug.DrawLine(transform.position, transform.position + (lastFrameRotation * Vector3.forward) * 5f, Color.red);

		if (isFireRecovery)
		{
			fireTimer += Time.deltaTime;
			if (fireTimer >= 1 / (weaponConfig.fireRate / 60))
			{
				isFireRecovery = false;
				fireTimer = 0;
			}
		}

	}

	/// <summary>
	/// Used when the player is firing at an ambiguous target
	/// </summary>
	public void Fire()
	{
		//Will override the regular fire procedure if an item is held
		if (isHoldingObject)
		{
			heldObject.transform.parent = null;
			Rigidbody rb = heldObject.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.isKinematic = false;
				rb.velocity = mainCam.transform.forward * heldObjectLaunchForce;
			}
			isHoldingObject = false;
			heldObject = null;


			return;
		}

		if (isReloading || currentAmmo <= 0 || isFireRecovery) return;



		//Random direction from the crosshair to move
		float randomAngle = UnityEngine.Random.Range(0f, 359f);
		float randomAngleRad = randomAngle * Mathf.Deg2Rad;

		float spreadValue = UnityEngine.Random.Range(0f, currentBaseSpread);


		//Find new screen coord to fire from.
		//Got clockwise angle already.
		//Hypotenuse is 1 due to unit length. Can use cos and sin directly
		Vector2 screenOffset = new Vector2(Mathf.Cos(randomAngleRad), Mathf.Sin(randomAngleRad)) * spreadValue; //Will eventually change to a value that is easier to change in the inspector
		Vector2 screenFirePos = new Vector2(Screen.width / 2, Screen.height / 2) + screenOffset;


		//Ray holds origin and direction, so this is the world direction of the spread value
		Ray fireRay = mainCam.ScreenPointToRay(screenFirePos);

		Vector3 fireDirection = (fireRay.origin + fireRay.direction) - mainCam.transform.position;
		fireDirection.Normalize();

		if (Physics.Raycast(mainCam.transform.position, fireDirection, out RaycastHit hit, weaponConfig.weaponRange, hittableLayers))
		{
			IDamagable damageable = hit.collider.GetComponent<IDamagable>();
			if (damageable != null)
			{
				damageable.TakeDamage(weaponConfig.weaponDamage);
			}

			GameObject impactMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
			impactMarker.transform.position = hit.point;
			impactMarker.transform.localScale = Vector3.one * 0.1f;
			impactMarker.GetComponent<Collider>().enabled = false;
			Destroy(impactMarker, 2f);
		}



		//Begin recovery and deduce ammo
		isFireRecovery = true;

		currentAmmo--;
		if (currentAmmo <= 0)
		{
			Reload();
		}

		WeaponFired?.Invoke(currentAmmo, reserveAmmo);

	}




	/// <summary>
	/// Used when the ai is firing at a target
	/// </summary>
	/// <param name="targetTransform"></param>
	/// <returns>True if hit an enemy</returns>
	public void Fire(Transform targetTransform)
	{
		if (isReloading || currentAmmo <= 0 || isFireRecovery) return;




		Ray fireRay = mainCam.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(cameraPos.transform.position, targetTransform.position - cameraPos.position, out RaycastHit hit, weaponConfig.weaponRange, hittableLayers))
		{
			IDamagable damageable = hit.collider.GetComponent<IDamagable>();
			if (damageable != null)
			{
				damageable.TakeDamage(weaponConfig.weaponDamage);
			}

		}




		isFireRecovery = true;

		currentAmmo--;
		if (currentAmmo <= 0)
		{
			Reload();
		}



		//WeaponFired?.Invoke(currentAmmo, reserveAmmo);
	}

	private void Reload()
	{
		if (isReloading)
		{
			return;
		}
		else if (currentAmmo != weaponConfig.maxAmmo)
		{
			isReloading = true;
		}


		StartCoroutine(ReloadRoutine());

	}

	private IEnumerator ReloadRoutine()
	{
		yield return new WaitForSeconds(1.5f);

		if (reserveAmmo > 0)
		{

			if (reserveAmmo >= weaponConfig.maxAmmo)
			{
				reserveAmmo -= weaponConfig.maxAmmo - currentAmmo;
				currentAmmo = weaponConfig.maxAmmo;
				UpdateAmmo(currentAmmo, reserveAmmo);
			}
			else
			{

				currentAmmo = reserveAmmo;
				reserveAmmo = 0;
				UpdateAmmo(currentAmmo, reserveAmmo);
			}
		}
		isReloading = false;

	}

	public void UpdateAmmo(int currentAmmo, int reserveAmmo)
	{
		WeaponFired?.Invoke(currentAmmo, reserveAmmo);
	}

	public void DisableControl()
	{
		inPlayerControl = false;
		InputManager.Instance.OnFirePressed -= Fire;
		InputManager.Instance.OnReloadPressed -= Reload;
	}

	public void EnableControl()
	{
		inPlayerControl = true;
		InputManager.Instance.OnFirePressed += Fire;
		InputManager.Instance.OnReloadPressed += Reload;


		UpdateAmmo(currentAmmo, reserveAmmo);
	}

	public void HoldItem(GameObject heldObject)
	{
		this.heldObject = heldObject;
		isHoldingObject = true;
	}

	public void SpreadMultiplierFromVelocity(Vector3 velocity, float maxSpeed)
	{
		float evaluatedCurve = speedSpreadCurve.Evaluate(velocity.magnitude / maxSpeed);

		movementSpreadMultiplier = 1 + (velocity.magnitude * movementMultiplierWeighting * evaluatedCurve);
	}
}
