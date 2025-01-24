using UnityEditor;
using UnityEngine;

public class Dreadbreaker : WeaponComponent
{
    [SerializeField]private float recoilPower;
    private GameObject _player;
    

    public override void Shoot(Vector3 targetPosition)
    {
        onShootStart.Invoke();
        Ammo--;
        Fire(targetPosition);
    }

    private void Fire(Vector3 targetPosition)
    {
        
        Recoil(targetPosition);
        Reload();
    }

    private void Recoil(Vector3 targetPosition)
    { 
        //if (PlayerController.isOnWall) return;
        
        _player.GetComponent<Rigidbody>().AddForce(-targetPosition * recoilPower, ForceMode.Impulse);
    }
}
