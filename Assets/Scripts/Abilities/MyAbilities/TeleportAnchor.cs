using UnityEngine;

[CreateAssetMenu(fileName = "TeleportAnchorAbility", menuName = "Abilities/Teleport Anchor")]
public class TeleportAnchor : CharacterAbility
{
	public GameObject anchorPrefab;
	public GameObject anchorProjectile;
	[SerializeField] private float travelDuration = 1.5f;
	[SerializeField] private float arcHeight = 5f;
	[SerializeField] private float launchForce = 20f;
	public float gadgetCount = 1;
	public bool anchorActive = false;
	public Vector3 teleportPosition;

	public override void Init()
	{
		gadgetCount = 1;
		teleportPosition = Vector3.zero;
		anchorActive = false;
	}	

	private void CreateAnchor(Vector3 position, Vector3 normal)
	{
		if (anchorPrefab != null)
		{
			GameObject anchor = Instantiate(anchorPrefab, position, Quaternion.LookRotation(-normal, Vector3.up));
			anchor.transform.up = normal;
			anchorActive = true;
			teleportPosition = anchor.transform.position + Vector3.up * 1.5f;
		}
	}

	public override void Use(AbilitySystem owner)
	{
		if (gadgetCount > 0)
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
			gadgetCount--;
		}
		else if (anchorActive == true)
		{
			CharacterController cont = owner.gameObject.GetComponent<CharacterController>();
			cont.enabled = false;
			owner.transform.position = teleportPosition;
			cont.enabled = true;

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
		
	}
}
