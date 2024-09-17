using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform  weaponHold;
    public Gun[] allGuns;
    Gun equippedGun; //«ˆ¿Á ¿Â¬¯¡ﬂ¿Œ √—

    public void EquipGun(Gun gunToEquip)
    {
        if(equippedGun != null)
            Destroy(equippedGun.gameObject);

        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        equippedGun.transform.parent = weaponHold;
    }

    public void EquipGun(int weaponIndex)
    {
        EquipGun(allGuns[weaponIndex]);
    }

    /// <summary>
    /// πÊæ∆ºË∏¶ ¥Á±Ë
    /// </summary>
    public void OnTriggerHold()
    {
        if(equippedGun != null)
            equippedGun.OnTriggerHold();
    }

    /// <summary>
    /// πÊæ∆ºË∏¶ ≥ı¿Ω
    /// </summary>
    public void OnTriggerRelease()
    {
        equippedGun.OnTriggerRelease();
    }

    /// <summary>
    /// ¡∂¡ÿ¡° πŸ∂Û∫Ω
    /// </summary>
    public void Aim(Vector3 aimPoint)
    {
        if(equippedGun != null)
        {
            equippedGun.Aim(aimPoint);
        }
    }

    /// <summary>
    /// ¿Á¿Â¿¸
    /// </summary>
    public void Reload()
    {
        if (equippedGun != null)
        {
            equippedGun.Reload();
        }
    }

    public float GunHeight
    {
        get
        {
            return weaponHold.position.y;
        }
    }
}
