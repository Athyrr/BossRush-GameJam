using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BossMvt : MonoBehaviour
{
    protected Boss _boss;

    [HideInInspector] public Vector3 Dir;
    [HideInInspector] public Rigidbody Rb;

    protected enum Phase { Chasing, Roaming };
    protected Phase _phase = Phase.Chasing;

    [SerializeField, Range(0, 360)] float minRoamAngleRange = 315;
    [SerializeField, Range(0, 360)] float maxRoamAngleRange = 345;
    private float _roamAngleRange;
    [SerializeField] float roamAngleMult = 4;
    [SerializeField, Range(0, 360)] float maxDirChangeAngle = 90;
    private Vector3 _lastDir;

    [SerializeField, Range(0, 100)] float ChaseChance = 70;
    [SerializeField, Range(0, 100)] float RoamChance = 50;
    private float _chaseTime;
    [SerializeField] private float _chaseMinDuration = 2;

    private void Awake()
    {
        _boss = GetComponent<Boss>();
        Rb = GetComponent<Rigidbody>();
        _lastDir = transform.forward;
    }

    private void Update()
    {
        switch (_phase)
        {
            case Phase.Chasing:
                ChaseDirUpdate();
                break;

            case Phase.Roaming:
                RoamRangeAngleUpdate();
                RoamDirUpdate();
                break;
        }
        ChangePhaseTry();
    }

    protected void ChaseDirUpdate()
    {
        Dir = (_boss.Target.position - transform.position).normalized;
        Dir.y = 0;
    }

    protected void RoamDirUpdate()
    {
        Vector3 initialRoamDir = (transform.position - _boss.Target.position).normalized;
        initialRoamDir.y = 0;

        float randomAngle = Random.Range(-maxDirChangeAngle / 2, maxDirChangeAngle / 2);

        Dir = Quaternion.AngleAxis(randomAngle, Vector3.up) * _lastDir;
        if (Vector3.Angle(initialRoamDir, Dir) > _roamAngleRange / 2)
            Dir = Quaternion.AngleAxis(_roamAngleRange / 2, Vector3.up) * initialRoamDir;
        else if (Vector3.Angle(initialRoamDir, Dir) < -_roamAngleRange / 2)
            Dir = Quaternion.AngleAxis(-_roamAngleRange / 2, Vector3.up) * initialRoamDir;

        _lastDir = Dir;
    }

    protected void RoamRangeAngleUpdate()
    {
        float dist = Vector3.Distance(transform.position, _boss.Target.position);

        if (dist < 10)
            _roamAngleRange = minRoamAngleRange;
        else if (dist > 50)
            _roamAngleRange = maxRoamAngleRange;
        else
            _roamAngleRange = minRoamAngleRange + dist * roamAngleMult;
    }

    protected virtual void ChangePhaseTry()
    {
        if (_chaseTime + _chaseMinDuration < Time.time && _phase != Phase.Roaming)
        {
            if (Vector3.Distance(transform.position, _boss.Target.position) < 20)
            {
                if (Random.Range(0, 100) < RoamChance)
                {
                    _phase = Phase.Roaming;
                    _chaseTime = Time.time;
                }
            }
        }
        else
        {
            if (_chaseTime + _chaseMinDuration < Time.time && Random.Range(0, 100) < ChaseChance && _phase != Phase.Chasing)
            {
                _phase = Phase.Chasing;
                _chaseTime = Time.time;
            }
        }
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, Dir * 1000);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, (transform.position - _boss.Target.position).normalized * 1000);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(_roamAngleRange / 2, Vector3.up) * (transform.position - _boss.Target.position).normalized * 1000);
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(-_roamAngleRange / 2, Vector3.up) * (transform.position - _boss.Target.position).normalized * 1000);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(maxDirChangeAngle / 2, Vector3.up) * _lastDir * 1000);
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(-maxDirChangeAngle / 2, Vector3.up) * _lastDir * 1000);
    }
    */
}
