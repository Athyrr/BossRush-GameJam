using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class WeaponComponent : MonoBehaviour
{
    private WeaponSO _weaponData;

    protected int Ammo;
    protected float Damage;
    private float _reloadDuration;
    protected float Range;

    private bool _isReloaded = true;

    public UnityEvent onEquipWeapon = new();
    public UnityEvent onUnequipWeapon = new();
    public UnityEvent onShootStart = new();
    public UnityEvent onShootEnd = new();
    public UnityEvent onShootUpdate = new();
    public UnityEvent onReloadStart = new();
    public UnityEvent onReloadEnd = new();

    private void Start()
    {
        Init();
    }

    protected void Init()
    {
        Ammo = _weaponData.maxAmmo;
        Damage = _weaponData.baseDamage;
        Range = _weaponData.range;
        _reloadDuration = _weaponData.reloadDuration;
    }
    public abstract void Shoot(Vector3 targetPosition);

    protected void Reload()
    {
        var timer = 0f;
        if (timer >= _reloadDuration)
            _isReloaded = true;
        else
        {
            timer += Time.deltaTime;
        }
    }
}