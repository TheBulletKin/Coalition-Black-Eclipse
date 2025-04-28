using System;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;

[CreateAssetMenu(fileName = "TeleportAnchorAbility", menuName = "Abilities/Teleport Anchor")]
public class TeleportAnchor : CharacterAbility
{
	[Header("Required prefabs")]
	[SerializeField] private GameObject anchorPrefab;
	[SerializeField] private GameObject anchorProjectile;
	[SerializeField] private TeleportAnchorObject anchorObject;

	[Header("Projectile attributes")]
	[SerializeField] private float travelDuration = 1.5f;	
	[SerializeField] private float launchForce = 20f;

	[Header("Debug")]
	[SerializeField] private bool anchorActive = false;
	[SerializeField] private bool anchorThrown = false;
	[SerializeField] private Vector3 teleportPosition;
	

	public override void Init(AbilitySystem owner)
	{
		base.Init(owner);		
		teleportPosition = Vector3.zero;
		anchorActive = false;
		anchorThrown = false;
	}	

	private void CreateAnchor(Vector3 position, Vector3 normal)
	{
		if (anchorPrefab != null)
		{
			GameObject anchor = Instantiate(anchorPrefab, position, Quaternion.LookRotation(-normal, Vector3.up));
			anchor.transform.up = normal;
			anchorObject = anchor.GetComponent<TeleportAnchorObject>();
			anchorActive = true;
			anchorThrown = false;

			anchorObject.SetPickupCallback(PickupAnchor);			
			
			teleportPosition = anchor.transform.position + Vector3.up * 1.5f;		

			GameEvents.OnGadgetPlaced?.Invoke(anchorObject);
		}
	}

	private void TeleportToAnchor(AbilitySystem owner)
	{
		CharacterController cont = owner.gameObject.GetComponent<CharacterController>();
		cont.enabled = false;
		owner.transform.position = teleportPosition;
		cont.enabled = true;
	}
	public override void Use(AbilitySystem owner)
	{
		if (currentAbilityCount > 0 && anchorActive == false && anchorThrown == false)
		{

			GameObject projectile = Instantiate(anchorProjectile, owner.GetCastposition() + owner.GetAimDirection() * 1.05f, Quaternion.identity);

			Rigidbody rb = projectile.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.velocity = owner.GetAimDirection() * launchForce;
			}

			TeleportAnchorProjectile projectileScript = projectile.GetComponent<TeleportAnchorProjectile>();
			if (projectileScript != null)
			{
				projectileScript.SetAnchorCallback(CreateAnchor);
			}
			currentAbilityCount--;
			anchorThrown = true;
		}
		else if (anchorActive == true && anchorThrown == false && anchorObject != null)
		{
			TeleportToAnchor(owner);
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

			if (anchorPrefab != null)
			{		
				CreateAnchor(targetVecPos, Vector3.up);
			}
			
			currentAbilityCount--;
		}
		else if (anchorActive == true)
		{
			TeleportToAnchor(owner);

		}
	}

	//Will need to adjust later to take in a specific anchor object when multiple anchors can be placed
	private void PickupAnchor()
	{
		currentAbilityCount = 1;
		anchorActive = false;
		teleportPosition = default;

		GameEvents.OnGadgetDestroyed(anchorObject);
		Destroy(anchorObject.gameObject);
	}
}
