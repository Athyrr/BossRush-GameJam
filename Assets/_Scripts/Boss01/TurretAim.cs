using UnityEngine;

public class TurretAim : MonoBehaviour
{
    private Vector3 _aimDir;
    [SerializeField] float _aimSpeed;
    [SerializeField] Boss _boss;

    void Update()
    {
        _aimDir = (_boss.Target.position - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, _aimDir, _aimSpeed / 100);
    }
}
