using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DaggerBlinkAbility", menuName = "Abilities/Dagger Blink")]
public class DaggerBlinkAbility : CharacterAbility, IGadget
{
	public GameObject daggerPrefab;
	public GameObject daggerProjectile;
	[SerializeField] private float travelDuration = 1.5f;
	[SerializeField] private float arcHeight = 5f;
	[SerializeField] private float launchForce = 40f;	
	public bool daggerActive = false;
	public bool daggerThrown = false;
	public Vector3 teleportPosition;
	private GameObject placedDagger;

	public Transform GadgetTransform => placedDagger.transform;

	public override void Init(AbilitySystem owner)
	{
		base.Init(owner);
		teleportPosition = Vector3.zero;
		daggerActive = false;
		daggerThrown = false;
	}

	private void CreateDagger(Vector3 position, Vector3 normal)
	{
		if (daggerPrefab != null)
		{
			placedDagger = Instantiate(daggerPrefab, position, Quaternion.LookRotation(-normal, Vector3.up));
			placedDagger.transform.up = normal;
			daggerActive = true;
			daggerThrown = false;
			teleportPosition = placedDagger.transform.position + Vector3.up * 1.5f;
		}
	}

	public override void Use(AbilitySystem owner)
	{
		if (currentAbilityCount > 0 && daggerThrown == false && daggerActive == false)
		{
			//Currently gets the components it needs through getComponent on the newly created projectile
			GameObject projectile = Instantiate(daggerProjectile, owner.GetCastposition() + owner.GetAimDirection() * 1.05f, Quaternion.identity);

			Rigidbody rb = projectile.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.velocity = owner.GetAimDirection() * launchForce;
			}

			DaggerBlinkProjectile projectileScript = projectile.GetComponent<DaggerBlinkProjectile>();
			if (projectileScript != null)
			{
				projectileScript.SetAnchorCallback(CreateDagger);
			}
			currentAbilityCount--;
			daggerThrown = true;

			GameEvents.OnGadgetPlaced?.Invoke(this);
		}
		else if (daggerActive == true)
		{
			CharacterController cont = owner.gameObject.GetComponent<CharacterController>();
			cont.enabled = false;
			owner.transform.position = teleportPosition;
			cont.enabled = true;			
			Destroy(placedDagger);
			placedDagger = null;
			daggerActive = false;
			GameEvents.OnGadgetPlaced?.Invoke(this);

		}
	}

	public override void Use(AbilitySystem owner, GameObject target)
	{

	}

	public override void Use(AbilitySystem owner, RaycastHit targetPos)
	{

	}

	public override void Use(AbilitySystem owner, Vector3 targetVecPos)
	{
		if (currentAbilityCount > 0)
		{

			if (daggerPrefab != null)
			{
				placedDagger = Instantiate(daggerPrefab, targetVecPos, Quaternion.identity);
				placedDagger.transform.up = Vector3.up;				
				teleportPosition = placedDagger.transform.position + Vector3.up * 1.5f;
			}

			currentAbilityCount--;
		}
		else if (daggerActive == true)
		{
			CharacterController cont = owner.gameObject.GetComponent<CharacterController>();
			cont.enabled = false;
			owner.transform.position = teleportPosition;
			cont.enabled = true;
			currentAbilityCount++;
			Destroy(placedDagger);
			placedDagger = null;
			daggerActive = false;

		}
	}
}

