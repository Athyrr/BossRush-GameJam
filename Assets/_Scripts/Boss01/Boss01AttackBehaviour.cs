using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Boss01AttackBehaviour : MonoBehaviour //BossAttackBehaviour
{
    private Boss01 _boss;
    [SerializeField] private WeaponShoot _turretWeapon;


    [Header("Turret Settings")]
    private int superShotGauge = 0;
    [SerializeField] private int superShotGaugeMax = 5;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject _superBullet;

    [Header("Ball Settings")]
    [SerializeField] private float BallDamage;

    private void Awake()
    {
        _boss = GetComponent<Boss01>();

    }

    private void Update()
    {
        if(_boss.animState.IsName("Turret"))
        {
            TurretBulletUpdate();
            _turretWeapon.Shoot();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(_boss.animState.IsName("Ball")) 
        {
            if (_boss.Mvt.Rb.linearVelocity.sqrMagnitude > 100 && other.transform.name == "target")
            {
                _boss.PlayerHit++;
                //repousser joueur et access son script pour baisser vie
                Debug.Log("hit!");
            }
        }
    }

    private void TurretBulletUpdate()
    {
        if(superShotGauge >= superShotGaugeMax)
        {
            _turretWeapon._bullet = _superBullet;
            superShotGauge = 0;
        }
        else
        {
            _turretWeapon._bullet = _bullet;
        }
    }
}
