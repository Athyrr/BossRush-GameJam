using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementComponent : MonoBehaviour
{
    [SerializeField]
    private EntityMovementSO _movementAsset = null;

    private Rigidbody _rigidbody = null;

    private float _maxSpeed = 1.0f;
    private float _speed = 1.0f;
    private float _smoothness = 1.0f;

    private Vector3 _previousMovement;


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

        if (_movementAsset == null)
        {
            Debug.LogError("MovementSO field is empty !");
            return;
        }

        Init();
    }

    public bool Move(Vector3 direction, float delta)
    {
        _speed = Mathf.Max(0, _speed);

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
        _rigidbody.position += velocity;

        _onMoveUpdate.Invoke(new MovementInfo(this, _speed, direction, transform.position));

        if (_previousMovement == Vector3.zero)
            _onMoveStart.Invoke(new MovementInfo(this, _speed, direction, previousPosition));

        return true;
    }

    private void Init()
    {
        _speed = _movementAsset.Speed;
        _maxSpeed = _movementAsset.MaxSpeed;

        _smoothness = _movementAsset.Smoothness;
    }
}
