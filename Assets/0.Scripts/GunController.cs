using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform  weaponHold;
    public Gun startGun;
    Gun equippedGun; //ÇöÀç ÀåÂøÁßÀÎ ÃÑ

    private void Start()
    {
        if (startGun != null)
            EquipGun(startGun);
    }

    public void EquipGun(Gun gunToEquip)
    {
        if(equippedGun != null)
            Destroy(equippedGun.gameObject);

        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        equippedGun.transform.parent = weaponHold;
    }

    /// <summary>
    /// ¹æ¾Æ¼è¸¦ ´ç±è
    /// </summary>
    public void OnTriggerHold()
    {
        if(equippedGun != null)
            equippedGun.OnTriggerHold();
    }

    /// <summary>
    /// ¹æ¾Æ¼è¸¦ ³õÀ½
    /// </summary>
    public void OnTriggerRelease()
    {
        equippedGun.OnTriggerRelease();
    }

    public void Aim(Vector3 aimPoint)
    {
        if(equippedGun != null)
        {
            equippedGun.Aim(aimPoint);
        }
    }

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
