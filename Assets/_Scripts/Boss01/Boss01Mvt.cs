using System.Collections;
using UnityEngine;

public class Boss01Mvt : BossMvt
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _fallSpeed = 5f;

    private void FixedUpdate()
    {
        if (_boss.IsGrounded(false))
            Move();
        Fall();
    }

    private void Move()
    {
       Rb.AddForce(Dir * _speed, ForceMode.VelocityChange);
    }

    private void Fall()
    {
        if(!_boss.IsGrounded(false))
        {
            Rb.AddForce(Rb.linearVelocity + new Vector3(0, -_fallSpeed, 0));
        }
    }
}
