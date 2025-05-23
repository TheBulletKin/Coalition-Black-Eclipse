using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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
	public bool inPlayerControl { get; private set; }
	private GameObject heldObject;
	private bool isHoldingObject;
	[SerializeField] private float heldObjectLaunchForce = 15f;
	[Tooltip("Angle of spread from the camera")]
	
	
	[SerializeField] public float currentBaseSpread = 1.0f;
	[SerializeField] public float crosshairConvergeanceRange = 20.0f;
	[SerializeField] private AnimationCurve speedSpreadCurve;
	[SerializeField] private PlayerMovementController playerMovementController;
	[SerializeField] private Quaternion lastFrameRotation;
	[SerializeField] private float rotationSpread;
	[SerializeField] private float rotationSpeed;
	private float smoothedAngleDiff = 0f;
	[SerializeField] private float angleSmoothingFactor = 0.1f;
	[SerializeField] private GameObject bulletOriginPos;
	[SerializeField] private GameObject audibleSoundPos;
	[SerializeField] private GameObject tracerPrefab;
	[SerializeField] private Sound impactSound;
	[SerializeField] private Material impactDecalMat;

	[SerializeField] private GameSoundSingle defaultGunshotSound;





	public event Action<ShootingSystem, int, int> OnWeaponFired;
	public event Action<ShootingSystem, int, int> OnUpdateAmmo;


	void Start()
	{
		mainCam = Camera.main;
		currentAmmo = weaponConfig ? weaponConfig.maxAmmo : 10;		
		isHoldingObject = false;
		UpdateAmmo(currentAmmo, reserveAmmo);
		playerMovementController = GetComponent<PlayerMovementController>();
		lastFrameRotation = transform.rotation;
		
	}

	// Update is called once per frame
	void Update()
	{

		float deltaTime = Time.deltaTime;

		HandleRotationSpread(deltaTime);


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

	private void HandleRotationSpread(float deltaTime)
	{
		if (inPlayerControl)
		{
			Quaternion currentRotation = mainCam.transform.rotation;

			float angleDifference = Quaternion.Angle(currentRotation, lastFrameRotation);

			//Since the rotation speed can be tiny and lost in frame by frame calculations, lerp picks up on changes over a slightly longer period
			//More slowly moves to the target speed over several frames
			smoothedAngleDiff = Mathf.Lerp(smoothedAngleDiff, angleDifference / deltaTime, angleSmoothingFactor);
			rotationSpeed = smoothedAngleDiff;

			if (rotationSpeed < 0.00001)
			{
				rotationSpeed = 0f;
			}

			//In order to use the animation curve properly, normalise from 0-1
			float normalizedRotationSpeed = Mathf.InverseLerp(0f, weaponConfig.maxRotationSpeed, rotationSpeed);
			//Curve result controls how much the angular velocity influences spread gain
			float curveValue = weaponConfig.rotationInfluenceCurve.Evaluate(normalizedRotationSpeed) * weaponConfig.rotationCurveMultiplier;

			//Only raise spread when above this minimum threshold
			if (rotationSpeed > weaponConfig.minRotSpeedForSpread)
			{
				rotationSpread += curveValue * weaponConfig.rotationInfluence * deltaTime;
			}
			else
			{
				//rotationSpread -= weaponConfig.rotationalSpreadDecreaseSpeed * deltaTime;
				rotationSpread = Mathf.MoveTowards(rotationSpread, 0f, weaponConfig.rotationalSpreadDecreaseSpeed * deltaTime);
			}

			rotationSpread = Mathf.Clamp(rotationSpread, 0f, weaponConfig.rotationSpreadMax);

			//Apply both movement and aim spread
			float totalAngle = (weaponConfig.baseSpreadAngle * weaponConfig.movementSpreadMultiplier) + (weaponConfig.baseSpreadAngle * rotationSpread);
			float spreadRadians = totalAngle * Mathf.Deg2Rad;

			//Determine the final spread from screen centre
			currentBaseSpread = Mathf.Tan(spreadRadians) * crosshairConvergeanceRange * 15f;

			lastFrameRotation = currentRotation;
		}
		else
		{
			currentBaseSpread = 0f;
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


		if (weaponConfig.isShotgun)
		{
			for (int i = 0; i < weaponConfig.pelletsPerShot; i++)
			{
				CastBulletRay(Mathf.RoundToInt(weaponConfig.weaponDamage / weaponConfig.pelletsPerShot));
			}
		}
		else
		{
			CastBulletRay(weaponConfig.weaponDamage);
		}


		//Begin recovery and deduce ammo
		isFireRecovery = true;

		rotationSpread += weaponConfig.spreadGainPerShot;

		currentAmmo--;
		if (currentAmmo <= 0)
		{
			Reload();
		}

		OnWeaponFired?.Invoke(this, currentAmmo, reserveAmmo);

		if (weaponConfig.gunfireSound != null && weaponConfig.emitsSound == true)
		{
			SoundEmitterHandler.instance.EmitAudibleSound(weaponConfig.gunfireSound, MixerBus.GUNSHOT, bulletOriginPos.transform.position, null);
			SoundEmitterHandler.instance.EmitDetectableSound(weaponConfig.gunfireAudibleSound, audibleSoundPos.transform.position);
		}
		else
		{
			SoundEmitterHandler.instance.EmitAudibleSound(defaultGunshotSound, MixerBus.GUNSHOT, bulletOriginPos.transform.position, null);
			SoundEmitterHandler.instance.EmitDetectableSound(weaponConfig.gunfireAudibleSound, audibleSoundPos.transform.position);
		}
	}

	private void CastBulletRay(int damage)
	{
		//Random direction from the crosshair to move
		float randomAngle = UnityEngine.Random.Range(0f, 359f);
		float randomAngleRad = randomAngle * Mathf.Deg2Rad;
		float spreadValue = UnityEngine.Random.Range(0f, currentBaseSpread);

		//Find new screen coord to fire from.
		//Got clockwise angle already.
		//Hypotenuse is 1 due to unit length. Can use cos and sin directly
		Vector2 screenOffset = new Vector2(Mathf.Cos(randomAngleRad), Mathf.Sin(randomAngleRad)) * spreadValue;

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
				damageable.TakeDamage(damage);

				//Temp to have the ai turn in the direction of impact

				Vector3 directionToCamera = (mainCam.transform.position - hit.point).normalized;
				Vector3 adjustedSoundPosition = hit.point + directionToCamera * 4f;
				SoundEmitterHandler.instance.EmitDetectableSound(impactSound, adjustedSoundPosition);

			}

			GameObject impactMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);
			impactMarker.transform.position = hit.point;
			impactMarker.transform.localScale = Vector3.one * 0.1f;
			impactMarker.GetComponent<Collider>().enabled = false;
			Renderer markerRenderer = impactMarker.GetComponent<Renderer>();
			if (markerRenderer != null && impactDecalMat != null)
			{
				markerRenderer.material = impactDecalMat;
			}
			Destroy(impactMarker, 2f);

			CreateTracer(bulletOriginPos.transform.position, hit.point);
		}
		else
		{
			Vector3 fallbackPoint = mainCam.transform.position + fireDirection * weaponConfig.weaponRange;
			CreateTracer(bulletOriginPos.transform.position, fallbackPoint);
		}


	}


	public void RequestFire()
	{
		if (CanFire())
		{
			Fire();
		}
	}

	public bool CanFire()
	{
		if (isReloading || currentAmmo <= 0 || isFireRecovery)
		{
			return false;
		}
		else
		{
			return true;
		}
	}


	/// <summary>
	/// Used when the ai is firing at a target
	/// </summary>
	/// <param name="targetTransform"></param>
	/// <returns>True if hit an enemy</returns>
	public void Fire(Transform targetTransform)
	{

		if (Physics.Raycast(cameraPos.transform.position, targetTransform.position - cameraPos.position, out RaycastHit hit, weaponConfig.weaponRange, hittableLayers))
		{
			IDamagable damageable = hit.collider.GetComponent<IDamagable>();
			if (damageable != null)
			{
				if (inPlayerControl)
				{
					damageable.TakeDamage(weaponConfig.weaponDamage);
				}
				else //For now, make it so that ai deal 2x damage. Players can headshot, allies can't. This will balance it for now but I'll need to consider another approach later
				{
					damageable.TakeDamage(weaponConfig.weaponDamage * 2);
				}
				
			}
			CreateTracer(bulletOriginPos.transform.position, hit.point);
		}




		isFireRecovery = true;

		OnWeaponFired?.Invoke(this, currentAmmo, reserveAmmo);

		currentAmmo--;
		if (currentAmmo <= 0)
		{
			Reload();
		}



		//WeaponFired?.Invoke(currentAmmo, reserveAmmo);
		if (weaponConfig.gunfireSound != null && weaponConfig.emitsSound == true)
		{
			SoundEmitterHandler.instance.EmitAudibleSound(weaponConfig.gunfireSound, MixerBus.GUNSHOT, bulletOriginPos.transform.position, null);
			SoundEmitterHandler.instance.EmitDetectableSound(weaponConfig.gunfireAudibleSound, audibleSoundPos.transform.position);
		}
		else
		{
			SoundEmitterHandler.instance.EmitAudibleSound(defaultGunshotSound, MixerBus.GUNSHOT, bulletOriginPos.transform.position, null);
			SoundEmitterHandler.instance.EmitDetectableSound(weaponConfig.gunfireAudibleSound, audibleSoundPos.transform.position);
		}


	}

	private void CreateTracer(Vector3 startPos, Vector3 endPos)
	{

		GameObject tracer = Instantiate(tracerPrefab, startPos, Quaternion.identity);
		TrailRenderer trail = tracer.GetComponent<TrailRenderer>();

		if (trail != null)
		{
			trail.Clear();
			tracer.transform.position = endPos;
			Destroy(tracer, trail.time);
		}
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
		yield return new WaitForSeconds(weaponConfig.reloadDuration);

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
		if (inPlayerControl)
		{
			//Call ammo update event only. Calling fire was causing issues when switching characters
			OnUpdateAmmo?.Invoke(this, currentAmmo, reserveAmmo);
		}

	}

	public void DisableControl()
	{
		inPlayerControl = false;
		InputManager.Instance.OnFirePressed -= RequestFire;
		InputManager.Instance.OnReloadPressed -= Reload;
	}

	public void EnableControl()
	{
		inPlayerControl = true;
		InputManager.Instance.OnFirePressed += RequestFire;
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

		weaponConfig.movementSpreadMultiplier = 1 + (velocity.magnitude * weaponConfig.movementMultiplierWeighting * evaluatedCurve);
	}

	public float GetAimTime(Vector3 target, Vector3 forward)
	{
		Vector3 directionToTarget = (target - transform.position).normalized;
		float angleToTarget = Vector3.Angle(forward, directionToTarget);
		float halfFiringAngle = weaponConfig.firingAngle * 0.5f;
		float centreAngleRange = weaponConfig.optimalAimCone * 0.5f;

		//Outside the vision cone
		if (angleToTarget > halfFiringAngle)
		{
			return 0f;
		}

		//In the sweet spot
		if (angleToTarget <= centreAngleRange)
		{
			return weaponConfig.aimTimeAtCentre;
		}

		//Determine aim time
		float outerRange = halfFiringAngle - centreAngleRange;
		float distanceFromCentre = angleToTarget - centreAngleRange;

		float t = distanceFromCentre / outerRange;
		return Mathf.Lerp(weaponConfig.aimTimeAtCentre, weaponConfig.aimTimeAtEdge, t);

	}

	private void OnDrawGizmosSelected()
	{
		if (weaponConfig != null)
		{
			Gizmos.color = Color.green;

			// Draw a wire sphere at the audible sound position with the configured radius
			Gizmos.DrawWireSphere(transform.position, weaponConfig.gunfireAudibleSound.soundRadius);
		}
	}
}
