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
    /// ��Ƽ踦 ���
    /// </summary>
    public void OnTriggerHold()
    {
        if (equippedGun != null)
        { 
            equippedGun.OnTriggerHold();
        }
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

    public Gun EquippedGun
    {
        get
        {
            return equippedGun;
        }
    }
}
