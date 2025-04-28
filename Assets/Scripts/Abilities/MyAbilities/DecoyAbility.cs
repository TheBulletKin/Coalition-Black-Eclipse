using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecoyAbility", menuName = "Abilities/Decoy")]
public class DecoyAbility : CharacterAbility, IGadget
{
	[SerializeField] private GameObject decoyPrefab;

	public Transform GadgetTransform => null;

	public override void Init(AbilitySystem owner)
	{
		base.Init(owner);
	}

	public override void Use(AbilitySystem owner)
	{

	}

	public override void Use(AbilitySystem owner, GameObject target)
	{

	}

	public override void Use(AbilitySystem owner, RaycastHit targetPos)
	{
		if (currentAbilityCount > 0)
		{
			GameObject newDecoy = Instantiate(decoyPrefab, targetPos.point, Quaternion.identity);
			//Ai use the entity manager to look for targets to check against, so the decoy should be added there
			EntityManager.Instance.AddNewDecoy(newDecoy.GetComponent<Decoy>());
			currentAbilityCount--;
			GameEvents.OnGadgetPlaced(this);
		}

	}

	public override void Use(AbilitySystem owner, Vector3 targetVecPos)
	{
		if (currentAbilityCount > 0)
		{
			GameObject newDecoy = Instantiate(decoyPrefab, targetVecPos, Quaternion.identity);
			EntityManager.Instance.AddNewDecoy(newDecoy.GetComponent<Decoy>());
			currentAbilityCount--;
			GameEvents.OnGadgetPlaced(this);
		}

	}
}
