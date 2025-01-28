using System.Collections;
using UnityEngine;

public class WeaponShoot : MonoBehaviour //purpose of this file = give to Fabien for his Weapon:MB script
{
    public GameObject _bullet; //need to modify this in Boss01AttackBehaviour
    private int _bounceCount;
    private float _bulletSpeed;

    private float _shotTime;
    [SerializeField] private float _shotCooldown = .2f;

    private void Awake()
    {
        //take _bullet, _bounceCount and _bulletSpeed via SO
    }
    public void Shoot()
    {
        if (_shotTime + _shotCooldown < Time.time)
        {
            _shotTime = Time.time;
            GameObject bullet = Instantiate(_bullet, transform.position, Quaternion.identity);
            int bounces = _bounceCount;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
            {
                StartCoroutine(SpawnBullet(bullet, hit, bounces));
            }
            //PS: you have to start a coroutine even if we hit nothing to make a bullet go anyway
        }
    }

    private IEnumerator SpawnBullet(GameObject bullet, RaycastHit hit, int bounceCount)
    {
        Vector3 startPos = bullet.transform.position;
        Vector3 dir = (hit.transform.position - bullet.transform.position).normalized;

        float dist = Vector3.Distance(startPos, hit.transform.position);
        float startDist = dist;

        //make the bullet travel the distance
        while (dist > 0)
        {
            bullet.transform.position = Vector3.Lerp(startPos, hit.transform.position, 1 - dist / startDist);
            dist -= _bulletSpeed * Time.deltaTime;
            yield return null;
        }

        bullet.transform.position = hit.transform.position;
        if (hit.transform.name == "target") // for debbuging. Replace with GetComponent<Damageable>().Damage(int) or smth like this
        {
            Destroy(bullet);
            Debug.Log("hit player!");
            yield break;
        }

        //bounce bullet recursively until count is 0
        if (bounceCount > 0)
        {
            Vector3 bounceDir = Vector3.Reflect(dir, hit.normal);
            if (Physics.Raycast(bullet.transform.position, bounceDir, out RaycastHit bounceHit))
                yield return StartCoroutine(SpawnBullet(bullet, bounceHit, bounceCount - 1));
        }
        Destroy(bullet);
    }
}
