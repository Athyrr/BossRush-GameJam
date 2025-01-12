using UnityEngine;

public class PlayerDashComponent : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private EntityDashSO _dashSettings = null;


    private float _dashForce = 0f;

    private float _coolddown = 0f;

    private float _dashDuration = 0f;

    private AnimationCurve _dashMomentum = null;

    private bool _allowDashInAir;

    private LayerMask _wallLayer;

    private LayerMask _enemyLayer;

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

    #endregion


    #region Lifecycle

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

    private void Init()
    {
        if (_dashSettings == null)
        {
            Debug.LogError("Dash settings not found.", this);
            return;
        }

        _dashForce = _dashSettings.DashForce;
        _coolddown = _dashSettings.Coolddown;
        _dashDuration = _dashSettings.Duration;
        _dashMomentum = _dashSettings.Momentum;
        _allowDashInAir = _dashSettings.AllowDashInAir;

        _wallLayer = _dashSettings.WallLayer;
        _enemyLayer = _dashSettings.EnemyLayer;
    }

    #endregion


    #region Public API
    public bool IsDashing => _dashTimer < _dashDuration && _isDashPerfomed;
    public EntityDashSO Settings => _dashSettings;

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

        if (DetectCollisions(direction, out RaycastHit hit))
        {
            _dashFinalPos = hit.point - direction * 0.2f;
        }

        _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);

        _dashTimer = 0;
        _runningCooldown = _coolddown;

        //invoke start dash event

        return true;
    }

    #endregion


    #region Private API

    private bool DetectCollisions(Vector3 direction, out RaycastHit hit)
    {
        if (Physics.Raycast(_dashOrigin, direction, out hit, _dashForce, _wallLayer))
            return true;

        return false;
    }


    #endregion


    #region Debug

    private void OnDrawGizmos()
    {
        if (_dashOrigin != Vector3.zero && _dashFinalPos != Vector3.zero)
        {
            Gizmos.color = _dashSettings.DebugColor;
            Gizmos.DrawLine(_dashOrigin, _dashFinalPos);
        }
    }

    #endregion
}
