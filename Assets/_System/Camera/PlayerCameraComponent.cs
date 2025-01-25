using UnityEngine;

public class PlayerCameraComponent : MonoBehaviour
{
    [SerializeField]
    private PlayerCameraSO _cameraSettings = null;

    [SerializeField]
    private Transform _target = null;

    [SerializeField]
    private Vector3 _offset = new Vector3(1.3f, 1, -3);

    private float _yaw;
    private float _pitch;

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

        Vector3 angles = transform.eulerAngles;
        _yaw = angles.y;
        _pitch = angles.x;

        //_offset = transform.position - _target.position;
    }

    public bool Look(Vector2 direction, float delta)
    {
        if (_target == null)
            return false;

        _yaw += direction.x * _cameraSettings.YawSensitivity * delta;
        _pitch -= direction.y * _cameraSettings.PitchSensitivity * delta;

        _pitch = Mathf.Clamp(_pitch, _cameraSettings.RotationLimits.x, _cameraSettings.RotationLimits.y);

        Quaternion desiredRotation = Quaternion.Euler(_pitch, _yaw, 0);
        Vector3 desiredPosition = _target.position + desiredRotation * _offset;

        transform.position = Vector3.Lerp(transform.position + _offset, desiredPosition, _cameraSettings.FollowSpeed * Time.deltaTime);
        transform.rotation = desiredRotation;

        return true;
    }

    private void OnDrawGizmos()
    {
        if (_target == null)
            return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_target.position, 0.2f);
    }
}
