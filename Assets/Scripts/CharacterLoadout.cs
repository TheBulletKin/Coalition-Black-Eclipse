using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterLoadout", menuName = "Characters/Loadout")]
public class CharacterLoadout : ScriptableObject
{
	[SerializeField] public string name;
	[SerializeField] public WeaponConfig primaryWeapon;
	[SerializeField] public WeaponConfig secondaryWeapon;
	//public List<Ability> abilities;
}
