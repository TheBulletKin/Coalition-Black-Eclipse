using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    Camera mainCam;
	[SerializeField] float weaponRange = 30.0f;
    [SerializeField] int weaponDamage = 45;
    [SerializeField] LayerMask hittableLayers;    
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;

        InputManager.Instance.OnFirePressed += Fire;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Fire()
    {
        Ray fireRay = mainCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit hit, weaponRange, hittableLayers))
        {
			IDamagable damageable = hit.collider.GetComponent<IDamagable>();
			if (damageable != null)
			{
				damageable.TakeDamage(weaponDamage);
			}
		}
    }
}
