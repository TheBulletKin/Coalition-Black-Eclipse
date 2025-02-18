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


	public override void Init()
	{
		gadgetCount = 1;
		teleportPosition = Vector3.zero;
		daggerActive = false;
	}
	public override void Use(AbilitySystem owner, GameObject target = null, RaycastHit targetPos = default)
	{
		if (gadgetCount > 0)
		{

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
}
