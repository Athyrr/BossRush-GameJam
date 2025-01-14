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
    private float _halfSize = 0;

    private UnityEvent<MovementInfo> _onMoveStart = new();
    private UnityEvent<MovementInfo> _onMoveUpdate = new();
    private UnityEvent<MovementInfo> _onMoveEnd = new();

    #endregion


    #region Lifecycle

    private void Awake()
    {
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();
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

        _halfSize = GetComponent<Collider>().bounds.extents.z;

        Init();
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
        _speed = Mathf.Clamp(_settings.Speed, 0, _settings.MaxSpeed);

        if ((direction == Vector3.zero && _player.IsGrounded) || _speed <= 0)
        {
            if (_previousMovement != Vector3.zero)
            {
                _onMoveEnd.Invoke(new MovementInfo(this, _speed, direction, transform.position));
                _previousMovement = Vector3.zero;
            }
            return false;
        }

        direction.Normalize();

        float control = _player.IsGrounded ? 1 : _settings.AirControl;

        Vector3 previousPosition = transform.position;
        Vector3 velocity = direction * _settings.Speed/* _speed */* control * delta;

        if (DetectCollisions(direction, out RaycastHit hit))
        {
            _rigidbody.position = hit.point - direction * _halfSize;


            Vector3 vel = _player.IsGrounded ?
                new Vector3(_rigidbody.linearVelocity.x, _rigidbody.linearVelocity.y, _rigidbody.linearVelocity.z)
                : new Vector3(_rigidbody.linearVelocity.x, Physics.gravity.y * _settings.FallingSpeedOnWall, _rigidbody.linearVelocity.z);

            _rigidbody.linearVelocity = vel;

            if (_previousMovement != Vector3.zero)
            {
                _onMoveEnd.Invoke(new MovementInfo(this, _speed, Vector3.zero, transform.position));
                _previousMovement = Vector3.zero;
            }
        }
        else
        {
            _rigidbody.position += velocity;
            _onMoveUpdate.Invoke(new MovementInfo(this, _speed, direction, transform.position));
        }


        if (_previousMovement == Vector3.zero)
            _onMoveStart.Invoke(new MovementInfo(this, _speed, direction, previousPosition));

        return true;
    }

    #endregion


    #region Private API

    private bool DetectCollisions(Vector3 direction, out RaycastHit hit)
    {
        if (Physics.Raycast(_rigidbody.position, direction, out hit, _halfSize + _settings.DetectionRange, _settings.WallLayer))
            return true;

        return false;
    }

    #endregion


    #region Debug

    private void OnDrawGizmos()
    {
        if (_rigidbody != null)
        {
            Gizmos.color = _settings.DebugColor;
            Gizmos.DrawWireSphere(_rigidbody.position, _halfSize + _settings.DetectionRange);
        }
    }

    #endregion
}
