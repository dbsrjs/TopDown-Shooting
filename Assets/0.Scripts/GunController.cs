using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform  weaponHold;
    public Gun startGun;
    Gun equipedGun; //ÇöÀç ÀåÂøÁßÀÎ ÃÑ

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

    /// <summary>
    /// ¹æ¾Æ¼è¸¦ ´ç±è
    /// </summary>
    public void OnTriggerHold()
    {
        if(equipedGun != null)
            equipedGun.OnTriggerHold();
    }

    /// <summary>
    /// ¹æ¾Æ¼è¸¦ ³õÀ½
    /// </summary>
    public void OnTriggerRelease()
    {
        equipedGun.OnTriggerRelease();
    }
}
