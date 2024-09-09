using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform  weaponHold;
    public Gun startGun;
    Gun equipedGun; //«ˆ¿Á ¿Â¬¯¡ﬂ¿Œ √—

    private void Start()
    {
        if (startGun != null)
            EquipGun(startGun);
    }

    public void EquipGun(Gun gunToEquip)
    {
        if(equipedGun != null)
            Destroy(equipedGun.gameObject);

        equipedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        equipedGun.transform.parent = weaponHold;
    }

    public void Shoot()
    {
        if(equipedGun != null)
        {
            equipedGun.Shoot();
        }
    }
}
