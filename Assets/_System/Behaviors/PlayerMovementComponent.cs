using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementComponent : MonoBehaviour
{
    [SerializeField]
    private EntityMovementSO _movementSettings = null;

    private Rigidbody _rigidbody = null;

    private float _maxSpeed = 1.0f;
    private float _speed = 1.0f;
    private float _smoothness = 1.0f;

    private Vector3 _previousMovement;

    private float _halfSize = 0;

    private UnityEvent<MovementInfo> _onMoveStart = new();
    private UnityEvent<MovementInfo> _onMoveUpdate = new();
    private UnityEvent<MovementInfo> _onMoveEnd = new();

    private void Awake()
    {
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //@todo Set settings about MoveSO.

        if (_movementSettings == null)
        {
            Debug.LogError("MovementSO field is empty !");
            return;
        }

        _halfSize = GetComponent<Collider>().bounds.extents.z;


        Init();
    }

    public bool Move(Vector3 direction, float delta)
    {
        _speed = Mathf.Clamp(_movementSettings.Speed, 0, _movementSettings.MaxSpeed);

        if (direction == Vector3.zero || _speed <= 0)
        {
            if (_previousMovement != Vector3.zero)
            {
                //@todo Invoke endMove event.
                _onMoveEnd.Invoke(new MovementInfo(this, _speed, direction, transform.position));
                _previousMovement = Vector3.zero;
            }
            return false;
        }

        direction.Normalize();

        Vector3 previousPosition = transform.position;
        Vector3 velocity = direction * _speed * delta;

        if (DetectCollisions(direction, out RaycastHit hit))
        {
            _rigidbody.position = hit.point - direction * _halfSize;
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, -_rigidbody.linearVelocity.y, _rigidbody.linearVelocity.z);
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

    private void Init()
    {
        _speed = _movementSettings.Speed;
        _maxSpeed = _movementSettings.MaxSpeed;

        _smoothness = _movementSettings.Smoothness;
    }

    private bool DetectCollisions(Vector3 direction, out RaycastHit hit)
    {
        if (Physics.Raycast(_rigidbody.position, direction, out hit, _halfSize + _movementSettings.DetectionRange, _movementSettings.WallLayer))
            return true;

        return false;
    }

    #region Debug

    private void OnDrawGizmos()
    {
        if (_rigidbody != null)
        {
            Gizmos.color = _movementSettings.DebugColor;
            Gizmos.DrawWireSphere(_rigidbody.position, _halfSize + _movementSettings.DetectionRange);
        }
    }

    #endregion

}
