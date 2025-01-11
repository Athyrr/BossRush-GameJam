using UnityEngine;

public class PlayerDashComponent : MonoBehaviour
{
    [SerializeField]
    private float _dashForce = 0f;

    [SerializeField]
    private float _coolddown = 0f;

    [SerializeField]
    private float _dashDuration = 0f;

    [SerializeField]
    private AnimationCurve _dashMomentum = null;


    private Rigidbody _rigidbody = null;

    /// <summary>
    /// Dash start postion.
    /// </summary>
    private Vector3 _dashOrigin = Vector3.zero;

    /// <summary>
    /// Estimated final dash position.
    /// </summary>
    private Vector3 _dashFinalPos = Vector3.zero;

    /// <summary>
    /// The time since the dash starts.
    /// </summary>
    private float _dashTimer = 0f;

    /// <summary>
    /// The current dash cooldown.
    /// </summary>
    private float _runningCooldown = 0f;

    private bool _isDashPerfomed = false;


    public bool IsDashing => _dashTimer < _dashDuration && _isDashPerfomed;


    private void Awake()
    {
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Init();
    }

    void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;

        if (IsDashing)
        {
            _dashTimer += Time.fixedDeltaTime;

            if (_dashTimer > _dashDuration)
            {
                delta = _dashTimer - _dashDuration;
                _dashTimer = _dashDuration;
            }

            float ratio = Mathf.Clamp01(_dashTimer / _dashDuration);

            float momentum = _dashMomentum.Evaluate(ratio);

            _rigidbody.MovePosition(Vector3.Lerp(_dashOrigin, _dashFinalPos, momentum));

            //if ratio >=1 end dash event
            if (ratio >= 1)
            {
                _isDashPerfomed = false;
                //invoke end dash
            }
        }

        if (_runningCooldown > 0)
            _runningCooldown -= delta;
    }

    public bool Dash(Vector3 direction)
    {

        if (IsDashing)
            return false;

        if (direction == Vector3.zero) //if allow while standing == false
            return false;

        if (_runningCooldown > 0)
            return false;

        direction.Normalize();

        _isDashPerfomed = true;

        _dashOrigin = _rigidbody.position;
        _dashFinalPos = _dashOrigin + direction * _dashForce;

        //_dashFinalPos.y = Mathf.Max(1, _dashFinalPos.y);

        _dashTimer = 0;
        _runningCooldown = _coolddown;

        //invoke start dash event

        return true;
    }

    private void Init()
    {
        // Allow dash while standing
    }


    private void OnDrawGizmos()
    {
        if (_dashOrigin != Vector3.zero && _dashFinalPos != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_dashOrigin, _dashFinalPos);
        }
    }
}
