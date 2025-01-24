using System;
using System.Buffers.Text;
using UnityEngine;

public class VoidFang : WeaponComponent
{
    private float _damage;
    private float _ammo;
    private float _durability;
    
    [SerializeField]private float speedMultiplier;
    
    private void Start()
    {
       Init();
    }

    protected override void Shoot()
    {
        onShootStart.Invoke();
        _ammo--;
    }

    protected override void Reload()
    {
        //
    }

    protected override void Init()
    {
        _damage = Damage;
        _ammo = Ammo;
        _durability = Durability;
    }

    protected override void CheckAmmo()
    {
        if (_ammo <= 0)
        {
            //Make disappear weapon
        }
    }
}
