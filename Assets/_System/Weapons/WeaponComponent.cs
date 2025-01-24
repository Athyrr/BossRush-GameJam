using UnityEngine;
using UnityEngine.Events;

    public abstract class WeaponComponent : MonoBehaviour
    {
        private WeaponSO _weaponData;

        protected int Ammo;
        protected int Damage;
        protected int Durability;

        public UnityEvent onEquipWeapon = new();
        public UnityEvent onUnequipWeapon = new();
        public UnityEvent onShootStart = new();
        public UnityEvent onShootEnd = new();
        public UnityEvent onShootUpdate = new();
        public UnityEvent onReloadStart = new();
        public UnityEvent onReloadEnd = new();


        protected abstract void Init();
        protected abstract void Shoot();
        
        protected abstract void Reload();
        
        protected abstract void CheckAmmo();
    }
    