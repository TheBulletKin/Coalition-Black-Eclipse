using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DaggerBlinkAbility", menuName = "Abilities/Dagger Blink")]
public class DaggerBlinkAbility : CharacterAbility
{
	public GameObject daggerPrefab;
	public GameObject daggerProjectile;
	[SerializeField] private float travelDuration = 1.5f;
	[SerializeField] private float arcHeight = 5f;
	[SerializeField] private float launchForce = 40f;
	public float gadgetCount = 1;
	public bool daggerActive = false;
	public Vector3 teleportPosition;
	private GameObject placedDagger;


	public override void Init(AbilitySystem owner)
	{
		gadgetCount = 1;
		teleportPosition = Vector3.zero;
		daggerActive = false;
	}

	private void CreateDagger(Vector3 position, Vector3 normal)
	{
		if (daggerPrefab != null)
		{
			placedDagger = Instantiate(daggerPrefab, position, Quaternion.LookRotation(-normal, Vector3.up));
			placedDagger.transform.up = normal;
			daggerActive = true;
			teleportPosition = placedDagger.transform.position + Vector3.up * 1.5f;
		}
	}

	public override void Use(AbilitySystem owner)
	{
		if (gadgetCount > 0)
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
			gadgetCount--;
			daggerActive = false;
		}
		else if (daggerActive == true)
		{
			CharacterController cont = owner.gameObject.GetComponent<CharacterController>();
			cont.enabled = false;
			owner.transform.position = teleportPosition;
			cont.enabled = true;
			gadgetCount++;
			Destroy(placedDagger);
			placedDagger = null;

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
		if (gadgetCount > 0)
		{

			if (daggerPrefab != null)
			{
				placedDagger = Instantiate(daggerPrefab, targetVecPos, Quaternion.identity);
				placedDagger.transform.up = Vector3.up;
				daggerActive = true;
				teleportPosition = placedDagger.transform.position + Vector3.up * 1.5f;
			}

			gadgetCount--;
		}
		else if (daggerActive == true)
		{
			CharacterController cont = owner.gameObject.GetComponent<CharacterController>();
			cont.enabled = false;
			owner.transform.position = teleportPosition;
			cont.enabled = true;
			gadgetCount++;
			Destroy(placedDagger);
			placedDagger = null;

		}
	}
}

