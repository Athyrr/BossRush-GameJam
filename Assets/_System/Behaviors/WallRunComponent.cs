using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class WallRunComponent : MonoBehaviour
{

    #region Fields

    //[SerializeField]
    //private WallRunSO _settings = null;

    [Header("Wallrun")]
    [SerializeField]
    private float _wallRunForce = 10f;

    [SerializeField]
    private float _wallRunDuration = 2f;

    [Header("Detection")]
    [SerializeField]
    private float _wallDetectionRange = 2f;

    [SerializeField]
    private LayerMask _wallRunnableLayer = ~0;

    [Header("Debug")]
    [SerializeField]
    private Color _canWallRunColor = Color.white;

    [SerializeField]
    private Color _cannotWallRunColor = Color.white;

    private Rigidbody _rigidbody = null;
    private float _halfWidth;
    private bool _isWallRunning = false;
    private float _wallRunTimer = 0f;
    private bool _canWallRunRight = false;
    private bool _canWallRunLeft = false;

    private float _initialYPosition = 0f;

    private bool _inputPressed = false;

    #endregion


    #region Lifecycle

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _halfWidth = GetComponent<Collider>().bounds.extents.z;

    }

    private void Update()
    {
        if (_inputPressed)
        {
            if (!TryStartWallRun(out Vector3 wallDir))
            {
                StopWallRun();
                return;
            }

            transform.forward = wallDir;

            _wallRunTimer += Time.deltaTime;
            _rigidbody.useGravity = false;
            MaintainHeight();

            Debug.Log("Wall Run");


            if (_wallRunTimer >= _wallRunDuration)
                StopWallRun();
        }
        else
        {
            StopWallRun();
        }

    }

    #endregion


    #region Public API

    public bool WallRun()
    {
        _inputPressed = true;
        return true;
    }

    #endregion


    #region Private API

    private bool TryStartWallRun(out Vector3 wallDirection)
    {
        wallDirection = Vector3.zero;

        if (CanWallRun(out Vector3 wallNormal))
        {
            wallDirection = wallNormal;
            StartWallRun(wallNormal);
            return true;
        }

        return false;
    }

    private void StartWallRun(Vector3 wallNormal)
    {
        if (_isWallRunning)
            return;

        _isWallRunning = true;
        _wallRunTimer = 0f;

        _initialYPosition = transform.position.y;

        Vector3 wallRunDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;
        if (Vector3.Dot(transform.forward, wallRunDirection) < 0)
        {
            wallRunDirection = -wallRunDirection;
        }

        transform.forward = wallRunDirection;
        _rigidbody.linearVelocity = wallRunDirection * _wallRunForce;

        _rigidbody.useGravity = false;
    }

    public void StopWallRun()
    {
        _inputPressed = false;
        if (!_isWallRunning)
            return;

        _rigidbody.useGravity = true;

        _isWallRunning = false;

        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.rotation = Quaternion.identity;
    }

    private void MaintainHeight()
    {
        Vector3 currentVelocity = _rigidbody.linearVelocity;
        _rigidbody.linearVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);

        Vector3 position = transform.position;
        position.y = _initialYPosition;
        transform.position = position;
    }

    private bool CanWallRun(out Vector3 wallNormal)
    {
        wallNormal = Vector3.zero;
        _canWallRunRight = false;
        _canWallRunLeft = false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + _halfWidth * transform.right, transform.right, out hit, _wallDetectionRange, _wallRunnableLayer))
        {
            wallNormal = hit.normal;
            _canWallRunRight = true;
            Debug.Log("Can Wall right");
            return true;
        }

        if (Physics.Raycast(transform.position - _halfWidth * transform.right, -transform.right, out hit, _wallDetectionRange, _wallRunnableLayer))
        {
            wallNormal = hit.normal;
            _canWallRunLeft = true;
            Debug.Log("Can Wall left");
            return true;
        }

        return false;
    }

    #endregion


    #region Debug

    private void OnDrawGizmos()
    {
        Gizmos.color = _canWallRunRight ? _canWallRunColor : _cannotWallRunColor;
        Gizmos.DrawLine(transform.position + _halfWidth * transform.right, transform.position + transform.right * _wallDetectionRange);

        Gizmos.color = _canWallRunLeft ? _canWallRunColor : _cannotWallRunColor;
        Gizmos.DrawLine(transform.position - _halfWidth * transform.right, transform.position - transform.right * _wallDetectionRange);
    }

    #endregion

}
