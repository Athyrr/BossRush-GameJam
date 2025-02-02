using UnityEngine;

public class Boss03Mvt : BossMvt
{
    public bool IsFlying = false;
    private float _flyTime;
    [SerializeField] private float _flyDuration;

    [SerializeField] private float _flySpeed;

    public int fury = 0;
    public int maxFury = 100;
    
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

        UpdateHeight();
        

        if(fury >= maxFury)
        {
            EnableFly();
        }

        if (_flyTime + _flyDuration < Time.time)
        {
            DisableFly();
        }

        //Debug
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EnableFly();
        }
    }

    private void FixedUpdate()
    {

        Move();
    }
    
    protected override void ChangePhaseTry()
    {
        if(Vector3.Distance(_boss.Target.position, transform.position) < 15 || IsFlying)
            _phase = Phase.Chasing;

        else
            _phase = Phase.Roaming;
    }

    private void Move() // remplacer par Move() de Adam
    {
        Rb.AddForce(Dir * 2, ForceMode.VelocityChange);
    }

    private void EnableFly()
    {
        IsFlying = true;
        _flyTime = Time.time;
        Rb.useGravity = false;
    }

    private void DisableFly()
    {
        IsFlying = false;
        Rb.useGravity = true;
        fury = 0;
    }
    private void UpdateHeight()
    {
        if (IsFlying && transform.position.y < 20)
        {
            transform.position = (transform.position + new Vector3(0, _flySpeed, 0));
        }

        else
        {
            Dir.y = 0;
        }
    }
}
