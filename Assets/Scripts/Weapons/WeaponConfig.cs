using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    public WeaponType weaponType;
    public string weaponName;

	[Header("Main Stats")]
    public int weaponDamage;
    public float weaponRange;
    public float firingAngle = 60;
    public int maxAmmo;
    [Tooltip("Fire rate in shots per minute")]
    public float fireRate;
    public bool isAutomatic;
	public float reloadDuration = 1.5f;
	public float spreadGainPerShot = 1f;
	public bool isShotgun = false;
	public float pelletsPerShot = 1f;
	public float aimTimeAtEdge;
	public float aimTimeAtCentre;
	public float optimalAimCone;

	[Header("Rotational Spread Values")]
	[Tooltip("How much of an effect rotation speed has on spread")]
	public float rotationInfluence;
	[Tooltip("How fast rotational spread decreases by itself")]
	public float rotationalSpreadDecreaseSpeed = 1f;
	[Tooltip("Maximum spread value")]
	public float rotationSpreadMax = 1f;
	[Tooltip("Rotational speed below which spread isn't applied")]
	public float minRotSpeedForSpread = 1f;	
	[Tooltip("Controls spread gain based on angular velocity")]
    public AnimationCurve rotationInfluenceCurve;
	[Tooltip("Multipliy the effects of spread gain through rotation speed")]
	public float rotationCurveMultiplier = 1f;
	[Tooltip("Compared with current rotation speed to create clamped 0-1 value for curve evaluation")]
	public float maxRotationSpeed = 1f;

	[Header("Sounds")]
	public GameSoundSingle gunfireSound;
	public bool emitsSound;
	public Sound gunfireAudibleSound;
    
    
}
