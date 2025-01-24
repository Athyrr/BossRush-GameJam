using System;
using System.Buffers.Text;
using UnityEngine;

public class VoidFang : WeaponComponent
{
    private float _damage;
    private float _ammo;
    private float _durability;
    
    [SerializeField]private float speedMultiplier;
    [SerializeField]private float dashDistanceMultiplier;
    
    private void Start()
    {
       Init();
    }

    public override void Shoot(Vector3 targetPositions)
    {
        onShootStart.Invoke();
        _ammo--;
    }
}
