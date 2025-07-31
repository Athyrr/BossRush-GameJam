using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementComponent : MonoBehaviour
{
    #region Nested
    public enum WalkMode { Gravity, Kinematic }
    #endregion

    #region Fields

    [SerializeField] private EntityMovementSO _settings = null;

    private PlayerComponent _player;
    private Rigidbody _rigidbody;
    private float _speed;
    private Vector3 _previousMovement = Vector3.zero;
    private float _halfWidth;
    private float _halfHeight;
    private Vector3 _surfaceNormal = Vector3.zero;
    private Quaternion _previousRotation;

    private bool _isGrounded = false;
    private bool _isOnWalkable = false;
    private WalkMode _walkMode = WalkMode.Gravity;
    private bool _isTouchingWall = false;
    private bool _isAiming = false;

    private Vector3 _movementDirection;
    private RaycastHit _surfaceHit;

    private readonly UnityEvent<MovementInfo> _onMoveStart = new();
    private readonly UnityEvent<MovementInfo> _onMoveUpdate = new();
    private readonly UnityEvent<MovementInfo> _onMoveEnd = new();

    #endregion

    #region Lifecycle

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = false;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
        if (_settings == null)
        {
            return;
        }

        if (!TryGetComponent(out _player))
        {
            Debug.LogError("PlayerComponent not found!");
            return;
        }

        Collider collider = GetComponent<Collider>();
        _halfWidth = collider.bounds.extents.x;
        _halfHeight = collider.bounds.extents.y;

        _speed = _settings.Speed;
    }

    private void FixedUpdate()
    {
        DetectSurface();
    }

    #endregion

    #region Public API

    public UnityEvent<MovementInfo> OnMoveStart => _onMoveStart;
    public UnityEvent<MovementInfo> OnMoveUpdate => _onMoveUpdate;
    public UnityEvent<MovementInfo> OnMoveEnd => _onMoveEnd;

    public bool Move(Vector3 direction, float delta, bool isAiming)
    {
        _isAiming = isAiming;

        if (direction == Vector3.zero)
        {
            StopMovement(delta);
            return false;
        }

        _speed = Mathf.Clamp(_settings.Speed, 0, _settings.MaxSpeed);
        direction.Normalize();
        _movementDirection = direction;

        switch (_walkMode)
        {
            case WalkMode.Gravity:
                MoveGravity(direction, delta);
                break;
            case WalkMode.Kinematic:
                WallRun(direction, delta);
                break;
        }

        Quaternion targetRotation = _isAiming
            ? Quaternion.LookRotation(Camera.main.transform.forward)
            : Quaternion.LookRotation(direction);

        _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, targetRotation, delta * _settings.RotationSpeed));

        if (_previousMovement == Vector3.zero)
            _onMoveStart.Invoke(new MovementInfo(this, _speed, direction, transform.position));

        _onMoveUpdate.Invoke(new MovementInfo(this, _speed, direction, transform.position));
        _previousMovement = direction;

        return true;
    }

    public void SwitchWalkMode(WalkMode newMode)
    {
        if (_walkMode == newMode) return;

        _walkMode = newMode;
        _rigidbody.isKinematic = newMode == WalkMode.Kinematic;
        _rigidbody.useGravity = newMode == WalkMode.Gravity;
    }

    #endregion

    #region Private API

    private void StopMovement(float delta)
    {
        SwitchWalkMode(WalkMode.Gravity);
        _rigidbody.angularVelocity = Vector3.zero;

        float verticalVelocity = _rigidbody.linearVelocity.y;
        Vector3 horizontalVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
        horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, _settings.DecelerationFactor * delta);

        _rigidbody.linearVelocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);

        if (_previousMovement != Vector3.zero)
        {
            _onMoveEnd.Invoke(new MovementInfo(this, _speed, Vector3.zero, transform.position));
            _previousMovement = Vector3.zero;
        }
    }

    private void DetectSurface()
    {
        Vector3 feetPosition = _rigidbody.position - transform.up * (_halfHeight - 0.2f);
        Collider[] colliders = Physics.OverlapSphere(feetPosition, _settings.WalkableDetectionRange, _settings.WalkableLayer);

        if (colliders.Length > 0 && Physics.Raycast(feetPosition, -transform.up, out _surfaceHit, _settings.WalkableDetectionRange, _settings.WalkableLayer))
        {
            _isOnWalkable = true;
            _surfaceNormal = _surfaceHit.normal;
            AlignToSurface(_surfaceNormal);

            if (Vector3.Dot(_surfaceHit.normal, Vector3.up) >= _settings.WallNormalThreshold)
            {
                SwitchWalkMode(WalkMode.Gravity);
                _isGrounded = true;
            }
            else
            {
                SwitchWalkMode(WalkMode.Kinematic);
                _isGrounded = false;
                _isTouchingWall = true;
            }
        }
        else
        {
            SwitchWalkMode(WalkMode.Gravity);
            _isGrounded = false;
            _isTouchingWall = false;
            _isOnWalkable = false;
        }
    }

    private void MoveGravity(Vector3 direction, float delta)
    {
        float control = _isGrounded ? 1f : _settings.AirControl;
        float aimStamp = _isAiming ? _settings.StampFactorOnAim : 1f;

        Vector3 velocity = direction * _settings.Speed * control * _settings.AcceleratonFactor * delta;
        velocity = Vector3.ProjectOnPlane(velocity, _surfaceNormal);

        _rigidbody.linearVelocity += velocity;
        Vector3 clamped = Vector3.ClampMagnitude(_rigidbody.linearVelocity, _settings.MaxSpeed);
        _rigidbody.linearVelocity = new Vector3(clamped.x, _rigidbody.linearVelocity.y, clamped.z) * aimStamp;
    }

    private void WallRun(Vector3 direction, float deltaTime)
    {
        Vector3 wallRunDirection = Vector3.ProjectOnPlane(direction, _surfaceNormal);
        Vector3 wallRunVelocity = wallRunDirection * _settings.Speed * deltaTime;

        AlignToSurface(_surfaceNormal);
        _rigidbody.MovePosition(_rigidbody.position + wallRunVelocity);

        MaintainPositionOnSurface(_surfaceHit);
    }

    private void MaintainPositionOnSurface(RaycastHit hit)
    {
        Vector3 targetPosition = hit.point + _surfaceNormal * 0.1f;

        if (_walkMode == WalkMode.Kinematic)
        {
            if (!Physics.CheckSphere(targetPosition, _halfWidth, _settings.WalkableLayer))
            {
                _rigidbody.position += targetPosition - _rigidbody.position;
            }
        }
        else
        {
            _rigidbody.MovePosition(targetPosition);
        }
    }

    private void AlignToSurface(Vector3 surfaceNormal)
    {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
        _rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, _settings.AlignementSpeed * Time.fixedDeltaTime));
    }

    #endregion

    #region Debug
    private void OnDrawGizmos()
    {
        if (_rigidbody == null || _settings == null) return;

        Vector3 feetPos = transform.position - transform.up * (_halfHeight - 0.1f);
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(feetPos, _settings.WalkableDetectionRange);

        Vector3 start = transform.position - transform.up * (_halfHeight * 0.5f) + transform.forward * _settings.ForwardDetectionOffset;
        Vector3 end = transform.position + transform.up * (_halfHeight * 0.5f) + transform.forward * _settings.ForwardDetectionOffset;

        Gizmos.color = _settings.ObstacleDetectionColor;
        Gizmos.DrawWireSphere(start, _halfWidth);
        Gizmos.DrawWireSphere(end, _halfWidth);
        Gizmos.DrawLine(start, end);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + _surfaceNormal * 2f);
    }
    #endregion
}
