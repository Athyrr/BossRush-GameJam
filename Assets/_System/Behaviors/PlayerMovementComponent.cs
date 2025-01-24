using UnityEngine;
using UnityEngine.Events;

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
        //AlignToSurface(_surfaceNormal);

        //if (DetectObstaclesAbove(out RaycastHit hit))
        //{
        //    MaintainPositionOnSurface(hit);
        //    Vector3 obstaclePosition = hit.point - transform.up * _halfHeight;
        //    _rigidbody.position = new Vector3(_rigidbody.position.x, obstaclePosition.y, _rigidbody.position.z);
        //    _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, Physics.gravity.y * _settings.FallingSpeedOnObstacles, _rigidbody.linearVelocity.z);
        //}

        Debug.Log("Move mode: " + _walkMode.ToString());
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

        if ((direction == Vector3.zero /*&&  _isGrounded*/))
        {
            //_rigidbody.useGravity = true;

            //if (!_isGrounded)
            //    SlideOnWall();

            // On ground

            float verticalVelocity = _rigidbody.linearVelocity.y;

            Vector3 horizontalVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, _settings.DecelerationFactor * delta); // 100 = Décélération

            _rigidbody.linearVelocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);
            //Debug.LogWarning("direction Input:" + direction);


            //On Wall
            //Set gravtity movement 

            if (_previousMovement != Vector3.zero)
            {
                _rigidbody.rotation = _previousRotation;
                _onMoveEnd.Invoke(new MovementInfo(this, _speed, direction, transform.position));
                _previousMovement = Vector3.zero;
            }
            return false;
        }

        _speed = Mathf.Clamp(_settings.Speed, 0, _settings.MaxSpeed);

        direction.Normalize();
        _movementDirection = direction;

        _rigidbody.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), delta * _settings.RotationSpeed);

        switch (_walkMode)
        {
            case WalkMode.Gravity:
                MoveGravity(direction, delta, out Vector3 velocity);
                break;

            case WalkMode.Kinematic:
                WallRun(direction, delta);
                break;
        }

        //if (DetectObstaclesFwd(direction, out RaycastHit hit))
        //{
        //    //ClampPositionToSurface(hit);
        //    if (direction == Vector3.zero)
        //    {
        //        _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, Physics.gravity.y * _settings.FallingSpeedOnObstacles, _rigidbody.linearVelocity.z);
        //        _rigidbody.position = hit.point - direction * _halfWidth;
        //    }
        //}

        if (_previousMovement != Vector3.zero)
        {
            _onMoveEnd.Invoke(new MovementInfo(this, _speed, Vector3.zero, transform.position));
            _previousMovement = Vector3.zero;
        }
        else
        {
            //_rigidbody.position += velocity;
            _onMoveUpdate.Invoke(new MovementInfo(this, _speed, direction, transform.position));
        }

        //Debug.DrawLine(_rigidbody.position, _rigidbody.position + targetPos * 10, Color.red);

        _previousRotation = _rigidbody.rotation;

        if (_previousMovement == Vector3.zero)
            _onMoveStart.Invoke(new MovementInfo(this, _speed, direction, transform.position));

        //Debug.Log("Is kinematic " + _rigidbody.isKinematic);

        return true;
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

        foreach (Collider col in colliders)
            Debug.Log("COlliders : " + col.name);

        if (detectBelow)
        {
            RaycastHit surfaceHit;
            if (Physics.Raycast(feetPosition, -transform.up, out surfaceHit, _settings.WalkableDetectionRange, _settings.WalkableLayer))
            {
                _isOnWalkable = true;
                _surfaceNormal = surfaceHit.normal;

                //MaintainPositionOnSurface(surfaceHit);
                //AlignToSurface(surfaceHit.normal);

                float dot = Vector3.Dot(surfaceHit.normal, Vector3.up);
                if (dot >= _settings.WallNormalThreshold)
                {
                    //MaintainPositionOnSurface(surfaceHit);
                    _isGrounded = true;
                    _walkMode = WalkMode.Gravity;
                }
                else
                {
                    //MaintainPositionOnSurface(surfaceHit);
                    _isGrounded = false;
                    _isTouchingWall = true;
                    _walkMode = WalkMode.Kinematic;
                }
            }
        }
        else
        {
            _isGrounded = false;
            _isTouchingWall = false;
            _isOnWalkable = false;
            _walkMode = WalkMode.Gravity;
        }

        //// Wall Detection
        //Vector3 startPoint = transform.position - transform.up * (_halfHeight * 0.5f) + transform.forward * _settings.ForwardDetectionOffset;
        //Vector3 endPoint = transform.position + transform.up * (_halfHeight * 0.5f) + transform.forward * _settings.ForwardDetectionOffset;

        //_isTouchingWall = Physics.CapsuleCast(
        //    startPoint,
        //    endPoint,
        //    _halfWidth,
        //    transform.forward,
        //    out RaycastHit wallHit,
        //    _settings.ObstacleDetectionRange,
        //    _settings.WalkableLayer
        //);


        //if (_isTouchingWall)
        //{
        //    float dot = Vector3.Dot(wallHit.normal, Vector3.up);

        //    if (dot < _settings.WallNormalThreshold && dot >= 0)
        //    {
        //        _walkMode = WalkMode.Kinematic;
        //        _surfaceNormal = wallHit.normal;
        //        //ClampPositionToSurface(wallHit);
        //        //AlignToGround(wallHit);
        //        _isOnWalkable = true;
        //    }
        //    else
        //    {
        //        _isTouchingWall = false;
        //    }
        //}

        //if (!_isGrounded && !_isTouchingWall)
        //{
        //    _isOnWalkable = false;
        //    _walkMode = WalkMode.Gravity;
        //}

        Debug.Log("Grounded: " + _isGrounded);
        //Debug.Log("Touching Wall: " + _isTouchingWall);
        //Debug.Log("On Walkable: " + _isOnWalkable);

    }

    private void MaintainPositionOnSurface(RaycastHit hit)
    {
        Vector3 offset = _rigidbody.position - hit.point;

        Vector3 projectedOffset = Vector3.Project(offset, hit.normal);

        if (projectedOffset.magnitude > 0.01f) 
        {
            Vector3 targetPosition = _rigidbody.position - projectedOffset;
            _rigidbody.MovePosition(Vector3.Lerp(_rigidbody.position, targetPosition, Time.fixedDeltaTime * 10f));
        }
    }

    private void AlignToSurface(Vector3 surfaceNormal)
    {
        //Quaternion targetRotation = Quaternion.LookRotation(Vector3.Cross(surfaceNormal, Vector3.up), surfaceNormal);
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, surfaceNormal) * transform.rotation;
        _rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, _settings.AlignementSpeed * Time.fixedDeltaTime));
    }

    private void SlideOnWall()
    {
        //Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, _currentSurfaceNormal).normalized;
        //float slideIntensity = Mathf.Clamp01(Vector3.Angle(Vector3.up, _currentSurfaceNormal) / 90f);
        //_rigidbody.AddForce(slideDirection * _settings.WallSlideSpeed * slideIntensity, ForceMode.Force);

        //Debug.Log("Wall Slide Active");
    }

    private void MoveGravity(Vector3 direction, float delta, out Vector3 velocity)
    {
        //velocity = Vector3.zero;
        //if (direction == Vector3.zero)
        //    return;

        float control = _isOnWalkable ? 1 : _settings.AirControl; ;

        velocity = direction * _settings.Speed * control * _settings.AcceleratonFactor * delta;
        velocity = Vector3.ProjectOnPlane(velocity, _surfaceNormal);

        AlignToSurface(_surfaceNormal);

        _rigidbody.linearVelocity += velocity;
        var clamped = Vector3.ClampMagnitude(_rigidbody.linearVelocity, _settings.MaxSpeed);
        _rigidbody.linearVelocity = new(clamped.x, _rigidbody.linearVelocity.y, clamped.z);

    }

    private void WallRun(Vector3 direction, float deltaTime)
    {
        Vector3 wallRunDirection = Vector3.ProjectOnPlane(direction, _surfaceNormal);
        Vector3 wallRunVelocity = wallRunDirection * _settings.Speed * deltaTime;

        _rigidbody.isKinematic = true;
        _rigidbody.MovePosition(_rigidbody.position + wallRunVelocity);

        AlignToSurface(_surfaceNormal);

        Debug.DrawLine(transform.position, transform.position + wallRunDirection * 5f, Color.green);
    }

    #endregion

    #region Debug

    private void OnDrawGizmos()
    {
        if (_rigidbody != null)
        {
            Vector3 feetPosition = transform.position - transform.up * (_halfHeight - 0.1f);

            // Ground
            float radius = _settings.WalkableDetectionRange;

            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(feetPosition, radius);

            // Wall
            Vector3 startPoint = transform.position - transform.up * (_halfHeight * 0.5f) + transform.forward * _settings.ForwardDetectionOffset;
            Vector3 endPoint = transform.position + transform.up * (_halfHeight * 0.5f) + transform.forward * _settings.ForwardDetectionOffset;

            Gizmos.color = _settings.ObstacleDetectionColor;
            Gizmos.DrawWireSphere(startPoint, _halfWidth);
            Gizmos.DrawWireSphere(endPoint, _halfWidth);
            Gizmos.DrawLine(startPoint, endPoint);

            // Surface Normal
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + _surfaceNormal * 2.0f);
        }
    }

    #endregion
}


