using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform  weaponHold;
    public Gun[] allGuns;
    Gun equippedGun; //���� �������� ��

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
    /// ��Ƽ踦 ���
    /// </summary>
    public void OnTriggerHold()
    {
        if(equippedGun != null)
            equippedGun.OnTriggerHold();
    }

    /// <summary>
    /// ��Ƽ踦 ����
    /// </summary>
    public void OnTriggerRelease()
    {
        equippedGun.OnTriggerRelease();
    }

    /// <summary>
    /// ������ �ٶ�
    /// </summary>
    public void Aim(Vector3 aimPoint)
    {
        if(equippedGun != null)
        {
            equippedGun.Aim(aimPoint);
        }
    }

    /// <summary>
    /// ������
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
