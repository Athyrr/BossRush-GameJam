using System;
using UnityEngine;

public class ShootComponent : MonoBehaviour
{
    [SerializeField] private HolsterComponent holster;


    public void Use(Vector3 targetPosition)
    {
        if (holster.currentWeapon == null)
        {
            return;
        }

        holster.currentWeapon.GetComponent<WeaponComponent>().Shoot(targetPosition);
    }
}
