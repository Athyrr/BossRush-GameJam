using UnityEngine;

public class PlayerCameraComponent : MonoBehaviour
{
    [SerializeField]
    private PlayerCameraSO _cameraSettings = null;

    [SerializeField]
    private Transform _target = null;


    private float _rotationSpeed = 0;

    private float _followSpeed = 0;

    private Vector2 _rotationLimits = Vector2.zero;

    private float _yaw;

    private float _pitch;


    Vector3 _offset = Vector3.zero;

    private void Start()
    {
        if (_cameraSettings == null)
        {
            Debug.LogError("Camera settings field is empty!");
            return;
        }

        if (_target == null)
        {
            Debug.LogError("Target field is empty!");
            return;
        }

        Init();

        Vector3 angles = transform.eulerAngles;
        _yaw = angles.y;
        _pitch = angles.x;

        _offset = transform.position - _target.position;
    }

    public bool Look(Vector2 direction, float delta)
    {
        if (_target == null)
            return false;

        direction.Normalize();

        _yaw += direction.x * _rotationSpeed * delta;
        _pitch -= direction.y * _rotationSpeed * delta;
        _pitch = Mathf.Clamp(_pitch, _rotationLimits.x, _rotationLimits.y);

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);
        Vector3 rotatedOffset = rotation * _offset;

        Vector3 targetPosition = _target.position + rotatedOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);

        transform.LookAt(_target.position + _offset.x * transform.right);

        return true;
    }

    private void Init()
    {
        _rotationSpeed = _cameraSettings.RotationSpeed;
        _followSpeed = _cameraSettings.FollowSpeed;
        _rotationLimits = _cameraSettings.RotationLimits;
    }

    private void OnDrawGizmos()
    {
        if (_target == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_target.position, _offset.magnitude);
    }
}
