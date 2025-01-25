using System;
using UnityEngine;

public class ShootComponent : MonoBehaviour
{
    //[SerializeField]
    //private ParticleSystem _ps = null;

    [SerializeField]
    private Transform _firePoint = null;

    public bool Shoot(Ray aimRay)
    {

        Debug.DrawRay(_firePoint.position, aimRay.direction * 50, Color.yellow, 0.5f);

        //_ps.Play();
        //_ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        if (Physics.Raycast(aimRay, out RaycastHit hit))
            Debug.Log(hit.collider.name);


        return true;
    }
}
