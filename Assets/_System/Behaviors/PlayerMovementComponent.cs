using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementComponent : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private EntityMovementSO _settings = null;

    private PlayerComponent _player = null;
    private Rigidbody _rigidbody = null;
    private float _speed = 1.0f;
    private Vector3 _previousMovement;
    private float _halfWidth = 0;
    private float _halfHeight = 0;
    private Vector3 _currentSurfaceNormal = Vector3.zero;
    private Quaternion _previousRotation;
    private bool _isGrounded = false;
    private bool _isOnWalkable;

    private UnityEvent<MovementInfo> _onMoveStart = new();
    private UnityEvent<MovementInfo> _onMoveUpdate = new();
    private UnityEvent<MovementInfo> _onMoveEnd = new();

    #endregion


    #region Lifecycle

    private void Awake()
    {
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.isKinematic = true;
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

        _halfWidth = GetComponent<Collider>().bounds.extents.z;
        _halfHeight = GetComponent<Collider>().bounds.extents.y;


        Init();
    }

    private void FixedUpdate()
    {
        DetectAndAlignToSurface();

        //if (DetectObstaclesAbove(out RaycastHit hit))
        //{
        //    Vector3 obstaclePosition = hit.point - Vector3.up * _halfHeight;
        //    _rigidbody.position = new Vector3(_rigidbody.position.x, obstaclePosition.y, _rigidbody.position.z);
        //    _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, Physics.gravity.y * _settings.FallingSpeedOnObstacles, _rigidbody.linearVelocity.z);
        //}

        //if (_rigidbody.useGravity == true)
        //{
        //    Debug.Log("Gravity: ON");
        //}
        //else
        //{
        //    Debug.Log("Gravity: OFF");
        //}

    }

    private void Init()
    {
        _speed = _settings.Speed;
    }

    #endregion


    #region Public API

    public UnityEvent<MovementInfo> OnMoveStart => _onMoveStart;

    public bool Move(Vector3 direction, float delta)
    {

        if ((direction == Vector3.zero && _isOnWalkable))
        {
            //_rigidbody.useGravity = true;

            //if (!_isGrounded)
            //    SlideOnWall();

            if (_previousMovement != Vector3.zero)
            {
                _rigidbody.rotation = _previousRotation;
                _onMoveEnd.Invoke(new MovementInfo(this, _speed, direction, transform.position));
                _previousMovement = Vector3.zero;
            }
            return false;
        }

        direction.Normalize();

        Debug.Log("Direction input" + direction);

        float control = /*_isOnWalkable ? 1 : _settings.AirControl;*/ 1;
        _speed = Mathf.Clamp(_settings.Speed, 0, _settings.MaxSpeed);

        Vector3 velocity = direction * _settings.Speed * control * delta;
        velocity = Vector3.ProjectOnPlane(velocity, _currentSurfaceNormal);

        var targetPos = _rigidbody.position + velocity;
        _rigidbody.MovePosition(_rigidbody.position + velocity);

        Debug.Log("Move target position" + velocity);

        //if (_isOnWalkable && !_isGrounded)
        //{
        //    if (_rigidbody.linearVelocity.magnitude > _settings.MaxSpeed)
        //    {
        //        _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * _settings.MaxSpeed;
        //    }

        //    Vector3 wallParallelDirection = Vector3.ProjectOnPlane(velocity, _currentSurfaceNormal);

        //    if (_rigidbody.linearVelocity.y > 00.2f)
        //    {
        //        _rigidbody.AddForce(wallParallelDirection * 10, ForceMode.Force);

        //    }

        //    //Vector3 antiGravityForce = -Physics.gravity * _rigidbody.mass;
        //    //_rigidbody.AddForce(antiGravityForce, ForceMode.Acceleration);

        //    //_rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, _currentSurfaceNormal);
        //    Debug.Log("OnWalll");
        //}


        //if (DetectObstaclesFwd(direction, out RaycastHit hit))
        //{
        //    _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, Physics.gravity.y * _settings.FallingSpeedOnObstacles, _rigidbody.linearVelocity.z);
        //    _rigidbody.position = hit.point - direction * _halfWidth;

        //    if (_previousMovement != Vector3.zero)
        //    {
        //        _onMoveEnd.Invoke(new MovementInfo(this, _speed, Vector3.zero, transform.position));
        //        _previousMovement = Vector3.zero;
        //    }
        //}
        //else
        //{
        //    _rigidbody.position += velocity;
        //    _onMoveUpdate.Invoke(new MovementInfo(this, _speed, direction, transform.position));
        //}

        //Debug.DrawLine(_rigidbody.position, _rigidbody.position + targetPos * 10, Color.red);

        //_previousRotation = _rigidbody.rotation;

        //if (_previousMovement == Vector3.zero)
        //    _onMoveStart.Invoke(new MovementInfo(this, _speed, direction, transform.position));

        Debug.Log("Is kinematic " + _rigidbody.isKinematic);

        return true;
    }

    #endregion


    #region Private API

    private bool DetectObstaclesFwd(Vector3 direction, out RaycastHit hit)
    {
        return Physics.Raycast(_rigidbody.position, direction, out hit, _halfWidth + _settings.ObstacleDetectionRange, _settings.ObstacleLayer);
    }

    private bool DetectObstaclesAbove(out RaycastHit hit)
    {
        return Physics.Raycast(transform.position, Vector3.up, out hit, _halfHeight + _settings.ObstacleDetectionRange, _settings.ObstacleLayer);
    }

    private void DetectAndAlignToSurface()
    {
        Vector3 rayOrigin = transform.position - transform.up * (_halfHeight);
        Vector3 direction = -transform.up;
        float distance = _halfHeight + _settings.WalkableDetectionRange;

        if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, distance, _settings.WalkableLayer))
        {
            _isOnWalkable = true;
            hit.normal.Normalize();
            _currentSurfaceNormal = Vector3.Lerp(_currentSurfaceNormal, hit.normal, Time.deltaTime * /*_settings.NormalSmoothingFactor*/ 5);
            //if (Vector3.Dot(Vector3.up, _currentSurfaceNormal) == 1)
            //{
            //    _isGrounded = true;
            //    _rigidbody.useGravity = true;
            //}
            //else
            //{
            //    _isGrounded = false;
            //    _rigidbody.useGravity = false;
            //}

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, _currentSurfaceNormal) * transform.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _settings.RotationSmoothness * Time.deltaTime);
        }
        else
        {
            //_isOnWalkable = false;
            //_isGrounded = false;
            //_rigidbody.useGravity = true;
            _currentSurfaceNormal = Vector3.up;
        }
    }

    private void SlideOnWall()
    {
        //Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, _currentSurfaceNormal).normalized;
        //float slideIntensity = Mathf.Clamp01(Vector3.Angle(Vector3.up, _currentSurfaceNormal) / 90f);
        //_rigidbody.AddForce(slideDirection * _settings.WallSlideSpeed * slideIntensity, ForceMode.Force);

        //Debug.Log("Wall Slide Active");
    }

    #endregion


    #region Debug

    private void OnDrawGizmos()
    {
        if (_rigidbody != null)
        {
            //Detection fwd
            Gizmos.color = _settings.ObstacleDetectionColor;
            Gizmos.DrawRay(_rigidbody.position, transform.forward * (_halfWidth + _settings.ObstacleDetectionRange));

            // Detection below
            Gizmos.color = _settings.WalkableDetectionColor;
            Vector3 rayOrigin = transform.position - transform.up * _halfHeight;
            Gizmos.DrawRay(rayOrigin, -transform.up * (_halfHeight + _settings.WalkableDetectionRange));


            //Detection above
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.up * (_halfHeight + _settings.ObstacleDetectionRange));

            //Surface normal
            Gizmos.color = Color.blue;
            Vector3 startPoint = transform.position - Vector3.up * _halfHeight;
            Gizmos.DrawLine(startPoint, startPoint + _currentSurfaceNormal * 5f);
        }
    }

    #endregion
}


