using System.Collections;
using UnityEngine;

public class TurretShoot : MonoBehaviour
{
    /*
    [SerializeField] private Transform _target;
    private Vector3 _dir;

    [SerializeField] private float _rotSpeed  = 5f;
    // variable of type PlayerScript with Damage() inside

    //shooting
    [Header("Bullet Settings")]
    [SerializeField] private int _bounceCount = 3;
    [SerializeField] private TrailRenderer _bulletTrail;
    [SerializeField] private TrailRenderer _superBulletTrail;
    [SerializeField] private float _bulletSpeed = 10;
    

    [Range(0,5)] private int superShotGauge = 0;

    private Boss01 _morphScript;

    private void Awake()
    {
        _morphScript = transform.parent.GetChild(0).GetComponent<Boss01>(); //pas faire ça
    }

    private void Update()
    {
        Aim();

        if(_shotTime + _shotCooldown < Time.time)
        {
            if(superShotGauge >= 5)
            {
                Debug.Log("Super!!");
                Shoot(true);
                superShotGauge = 0;
            }
            else
                Shoot(false);
            _shotTime = Time.time;
        }
    }

    private void Shoot(bool super)
    {
        TrailRenderer bullet;
        int bounces;

        if (super)
        {
            bullet = Instantiate(_superBulletTrail, transform.position, Quaternion.identity);
            bounces = 0;
        }
        else
        {
            bullet = Instantiate(_bulletTrail, transform.position, Quaternion.identity);
            bounces = _bounceCount;
        }

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            StartCoroutine(SpawnBullet(bullet, hit, bounces));
        }
    }

    private IEnumerator SpawnBullet(TrailRenderer bullet, RaycastHit hit, int bounceCount)
    {
        Vector3 startPos = bullet.transform.position;
        Vector3 dir = (hit.transform.position - bullet.transform.position).normalized;

        float dist = Vector3.Distance(startPos, hit.transform.position);
        float startDist = dist;

        while(dist > 0)
        {
            bullet.transform.position = Vector3.Lerp(startPos, hit.transform.position, 1 - dist / startDist);
            dist -= _bulletSpeed * Time.deltaTime;
            yield return null;
        }

        bullet.transform.position = hit.transform.position;
        if(hit.transform.name == "target")
        {
            _morphScript.PlayerHit++;
            superShotGauge++;
            Destroy(bullet);
            Debug.Log("hit!");
            yield break;
        }

        if(bounceCount > 0)
        {
            Vector3 bounceDir = Vector3.Reflect(dir, hit.normal);
            if (Physics.Raycast(bullet.transform.position, bounceDir, out RaycastHit bounceHit))
                yield return StartCoroutine(SpawnBullet(bullet, bounceHit, bounceCount - 1));
        }
        Destroy(bullet);
    }

    private void Aim()
    {
        _dir = (_target.position - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, _dir, _rotSpeed / 100);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 1000);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, _dir * 1000);
    }
    */
}
