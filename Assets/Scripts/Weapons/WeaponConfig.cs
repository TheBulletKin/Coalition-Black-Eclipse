using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    public WeaponType weaponType;
    public string weaponName;
    public int weaponDamage;
    public float weaponRange;
    public float firingAngle = 60;
    public int maxAmmo;
    [Tooltip("Fire rate in shots per minute")]
    public float fireRate;
    public bool isAutomatic;
}
