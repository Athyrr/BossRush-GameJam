using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Boss01AttackBehaviour : MonoBehaviour
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
        _turretWeapon.Bullet = _bullet;

    }

    private void Update()
    {
        if(_boss.animState.IsName("Turret") && _boss.CanShoot)
        {
            if (_turretWeapon.Shoot()) //bon il croit toucher à chaque fois le joueur du coup faut que je fix ça
            {
                TurretBulletUpdate();
                _boss.PlayerHit++;
            }
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
        superShotGauge++;
        if(superShotGauge >= superShotGaugeMax)
        {
            _turretWeapon.Bullet = _superBullet;
            superShotGauge = 0;
        }
        else
        {
            _turretWeapon.Bullet = _bullet;
        }
    }
}
