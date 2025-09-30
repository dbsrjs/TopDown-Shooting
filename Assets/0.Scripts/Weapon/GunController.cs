using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform  weaponHold;
    public Gun[] allGuns;
    Gun equippedGun; //현재 장착된 총

    public void EquipGun(Gun gunToEquip)
    {
        if(equippedGun != null)
            Destroy(equippedGun.gameObject);

        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        equippedGun.transform.parent = weaponHold;
    }

    public void EquipGun(int weaponIndex)
    {
        // 인덱스 범위 확인
        if (weaponIndex >= 0 && weaponIndex < allGuns.Length)
        {
            EquipGun(allGuns[weaponIndex]);
        }
        else
        {
            Debug.LogWarning($"무기 인덱스 {weaponIndex}가 범위를 벗어났습니다. 무기 배열 크기: {allGuns.Length}");
        }
    }

    /// <summary>
    /// 트리거를 누름
    /// </summary>
    public void OnTriggerHold()
    {
        if (equippedGun != null)
        { 
            equippedGun.OnTriggerHold();
        }
    }

    /// <summary>
    /// 트리거를 놓음
    /// </summary>
    public void OnTriggerRelease()
    {
        equippedGun.OnTriggerRelease();
    }

    /// <summary>
    /// 조준점을 바라봄
    /// </summary>
    public void Aim(Vector3 aimPoint)
    {
        if(equippedGun != null)
        {
            equippedGun.Aim(aimPoint);
        }
    }

    /// <summary>
    /// 재장전
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

    public Gun EquippedGun
    {
        get
        {
            return equippedGun;
        }
    }
}
