using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementComponent : MonoBehaviour
{

    #region Nested

    public enum WalkMode
    {
        Gravity,
        Kinematic
    }
    #endregion

    #region Fields

    [SerializeField]
    private EntityMovementSO _settings = null;

    private PlayerComponent _player = null;
    private Rigidbody _rigidbody = null;
    private float _speed = 1.0f;
    private Vector3 _previousMovement;
    private float _halfWidth = 0;
    private float _halfHeight = 0;
    private Vector3 _surfaceNormal = Vector3.zero;
    private Quaternion _previousRotation;
    private bool _isGrounded = false;

    private bool _isOnWalkable;
    private WalkMode _walkMode = WalkMode.Gravity;

    private bool _isTouchingWall = false;

    private UnityEvent<MovementInfo> _onMoveStart = new();
    private UnityEvent<MovementInfo> _onMoveUpdate = new();
    private UnityEvent<MovementInfo> _onMoveEnd = new();
    private Vector3 _movementDirection;
    private RaycastHit _surfaceHit;
    private bool _isAiming;

    #endregion


    #region Lifecycle

    private void Awake()
    {
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.isKinematic = false;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Start()
    {
        if (_settings == null)
        {
            Debug.LogError("MovementSO field is empty !");
            return;
        }

        if (!TryGetComponent<PlayerComponent>(out _player))
        {
            Debug.LogError("player component not found!");
            return;
        }

        var collider = GetComponent<Collider>();
        _halfWidth = collider.bounds.extents.x;
        _halfHeight = collider.bounds.extents.y;


        Init();
    }

    private void FixedUpdate()
    {
        DetectSurface();
    }

    private void Init()
    {
        _speed = _settings.Speed;
    }

    #endregion

    #region Public API

    public UnityEvent<MovementInfo> OnMoveStart => _onMoveStart;

    public bool Move(Vector3 direction, float delta, bool isAiming)
    {
        _isAiming = isAiming;

        if (direction == Vector3.zero)
        {

            SwitchWalkMode(WalkMode.Gravity);
            _rigidbody.angularVelocity = Vector3.zero;

            float verticalVelocity = _rigidbody.linearVelocity.y;

            Vector3 horizontalVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, _settings.DecelerationFactor * delta);

            _rigidbody.linearVelocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);

            if (_previousMovement != Vector3.zero)
            {
                _onMoveEnd.Invoke(new MovementInfo(this, _speed, direction, transform.position));
                _previousMovement = Vector3.zero;
            }
            return false;
        }

        _speed = Mathf.Clamp(_settings.Speed, 0, _settings.MaxSpeed);

        direction.Normalize();

        //direction = Vector3.ProjectOnPlane(transform.up, _surfaceNormal);

        _movementDirection = direction;

        switch (_walkMode)
        {
            case WalkMode.Gravity:
                MoveGravity(direction, delta, out Vector3 velocity);
                break;

            case WalkMode.Kinematic:
                WallRun(direction, delta);
                break;
        }

        if (isAiming)
            _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, Quaternion.LookRotation(Camera.main.transform.forward), delta * _settings.RotationSpeed));
        else
            _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, Quaternion.LookRotation(direction), delta * _settings.RotationSpeed));

        if (_previousMovement != Vector3.zero)
        {
            _onMoveEnd.Invoke(new MovementInfo(this, _speed, Vector3.zero, transform.position));
            _previousMovement = Vector3.zero;
        }
        else
        {
            _onMoveUpdate.Invoke(new MovementInfo(this, _speed, direction, transform.position));
        }

        _previousRotation = _rigidbody.rotation;

        if (_previousMovement == Vector3.zero)
            _onMoveStart.Invoke(new MovementInfo(this, _speed, direction, transform.position));

        return true;
    }

    public void SwitchWalkMode(WalkMode newMode)
    {
        if (_walkMode == newMode) return;

        _walkMode = newMode;
        if (_walkMode == WalkMode.Gravity)
        {
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
        }
        else if (_walkMode == WalkMode.Kinematic)
        {
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
        }
    }

    #endregion

    #region Private API

    private bool DetectObstaclesFwd(Vector3 direction, out RaycastHit hit)
    {
        return Physics.Raycast(_rigidbody.position, direction, out hit, _halfHeight + _settings.ObstacleDetectionRange, _settings.ObstacleLayer);
    }

    private bool DetectObstaclesAbove(out RaycastHit hit)
    {
        return Physics.Raycast(_rigidbody.position, Vector3.up, out hit, _halfHeight + 0.1f, _settings.ObstacleLayer);
    }

    private void DetectSurface()
    {
        Vector3 feetPosition = _rigidbody.position - transform.up * (_halfHeight - 0.2f);
        float radius = _settings.WalkableDetectionRange;

        Collider[] colliders = Physics.OverlapSphere(feetPosition, radius, _settings.WalkableLayer);

        bool detectBelow = colliders.Length > 0;

        if (detectBelow)
        {
            if (Physics.Raycast(feetPosition, -transform.up, out _surfaceHit, _settings.WalkableDetectionRange, _settings.WalkableLayer))
            {
                _isOnWalkable = true;
                _surfaceNormal = _surfaceHit.normal;

                AlignToSurface(_surfaceHit.normal);

                float dot = Vector3.Dot(_surfaceHit.normal, Vector3.up);
                if (dot >= _settings.WallNormalThreshold)
                {

                    SwitchWalkMode(WalkMode.Gravity);
                    _isGrounded = true;

                    //Debug.Log("On Ground");
                }
                else
                {
                    SwitchWalkMode(WalkMode.Kinematic);
                    _isGrounded = false;
                    _isTouchingWall = true;

                    //Debug.Log("On Wall");
                }
            }
        }
        else
        {
            //Debug.Log("On air");
            SwitchWalkMode(WalkMode.Gravity);
            _isGrounded = false;
            _isTouchingWall = false;
            _isOnWalkable = false;

            //transform.up = Vector3.up;
            //transform.forward = _movementDirection;
        }
    }

    private void MaintainPositionOnSurface(RaycastHit hit)
    {
        Vector3 targetPosition = hit.point + _surfaceNormal * 0.1f;

        if (_walkMode == WalkMode.Kinematic)
        {
            if (!Physics.CheckSphere(targetPosition, _halfWidth, _settings.WalkableLayer))
            {
                Vector3 displacement = targetPosition - _rigidbody.position;
                _rigidbody.position += displacement;
            }
            else
            {
                Debug.Log("Collision detected at target position. Movement cancelled.");
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

    private void SlideOnWall()
    {
    }

    private void MoveGravity(Vector3 direction, float delta, out Vector3 velocity)
    {
        float control = _isGrounded ? 1 : _settings.AirControl;

        float aimStamp = _isAiming ? _settings.StampFactorOnAim : 1;

        velocity = direction * _settings.Speed * control * _settings.AcceleratonFactor * delta;
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

        _rigidbody.isKinematic = true;
        _rigidbody.MovePosition(_rigidbody.position + wallRunVelocity);

        MaintainPositionOnSurface(_surfaceHit);

        Debug.DrawLine(transform.position, transform.position + wallRunDirection * 5f, Color.green);
    }

    #endregion

    #region Debug

    private void OnDrawGizmos()
    {
        if (_rigidbody != null)
        {
            Vector3 feetPosition = transform.position - transform.up * (_halfHeight - 0.1f);

            float radius = _settings.WalkableDetectionRange;

            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(feetPosition, radius);

            Vector3 startPoint = transform.position - transform.up * (_halfHeight * 0.5f) + transform.forward * _settings.ForwardDetectionOffset;
            Vector3 endPoint = transform.position + transform.up * (_halfHeight * 0.5f) + transform.forward * _settings.ForwardDetectionOffset;

            Gizmos.color = _settings.ObstacleDetectionColor;
            Gizmos.DrawWireSphere(startPoint, _halfWidth);
            Gizmos.DrawWireSphere(endPoint, _halfWidth);
            Gizmos.DrawLine(startPoint, endPoint);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + _surfaceNormal * 2.0f);
        }
    }

    #endregion
}
