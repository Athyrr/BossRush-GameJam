using UnityEngine;

    [CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/WeaponSO")]
    public class WeaponSO : ScriptableObject
    {
        public float baseDamage;
        public int maxAmmo;
        public float reloadDuration;
        public float range;
    }
