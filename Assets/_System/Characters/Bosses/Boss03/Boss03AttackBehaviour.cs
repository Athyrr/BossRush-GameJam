using System.Collections;
using UnityEngine;

public class Boss03AttackBehaviour : MonoBehaviour
{
    private Boss _boss;
    private Boss03Mvt _mvt;

    private float AttackTime;
    [SerializeField] private float AttackCooldown;

    [SerializeField] private GameObject _bomb;

    private Coroutine _coroutine;
    private void Awake()
    {
        _boss = GetComponent<Boss>();
        _mvt = GetComponent<Boss03Mvt>();
    }

    private void Update()
    {
        //if already attacking or cooldown active, return
        if (_coroutine != null || AttackTime + AttackCooldown > Time.time)
            return;
        
        if (_mvt.IsFlying)
        {
            if (Vector3.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(_boss.Target.position.x, _boss.Target.position.z)) < 5)
            {
                _coroutine = StartCoroutine(BombAttack());
                AttackTime = Time.time;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, _boss.Target.position) < 10)
            {
                _coroutine = StartCoroutine(MeleeAttack());
                AttackTime = Time.time;
            }

            else if (Vector3.Distance(transform.position, _boss.Target.position) > 30)
            {
                _coroutine = StartCoroutine(RangedAttack());
                AttackTime = Time.time;
            }
        }
    }

    private IEnumerator MeleeAttack()
    {
        _boss.Mvt.enabled = false;
        Debug.Log("Melee");
        //trigger melee anim
        //access player's damageable
        yield return new WaitForSeconds(2);
        _boss.Mvt.enabled = true;
        _coroutine = null;
    }

    private IEnumerator RangedAttack()
    {
        _boss.Mvt.enabled = false;
        Debug.Log("Ranged");
        //trigger melee anim
        //access player's damageable
        yield return new WaitForSeconds(2);
        _boss.Mvt.enabled = true;
        _coroutine = null;
    }

    private IEnumerator BombAttack()
    {
        Debug.Log("Bomb");
        //Instantiate(_bomb, transform.position, Quaternion.identity); //ou faire une weapon? on peut utiliser le SpawnBullet coroutine mais avec une bombe
        yield return new WaitForSeconds(2);
        _coroutine = null;
    }
}
