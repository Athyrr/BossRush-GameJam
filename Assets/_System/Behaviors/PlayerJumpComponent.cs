using UnityEngine;

public class PlayerJumpComponent : MonoBehaviour
{
    [SerializeField]
    private EntityJumpSO _jumpSettings = null;

    [SerializeField]
    private float jumpForce = 10f;

    [SerializeField]
    private float gravityMultiplier = 2f;

    [SerializeField]
    private float coyoteTime = 0.2f;

    [SerializeField]
    private float maxFallingSpeed = 10f;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private float _groundDetectionRange = 0.2f;

    private Rigidbody _rigidbody = null;
    private float _halfHeight = 0;

    private float coyoteTimeCounter;
    private bool _isGrounded;


    private void Awake()
    {
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (_jumpSettings == null)
        {
            Debug.LogError("Jump settigns field is empty !");
            return;
        }

        _halfHeight = GetComponent<Collider>().bounds.extents.y;

        Init();
    }

    private void Update()
    {
        CheckGrounded();
        HandleCoyoteTime(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        HandleFalling();
    }

    private void HandleFalling()
    {
        if (_isGrounded)
            return;

        _rigidbody.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
        _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, Mathf.Max(_rigidbody.linearVelocity.y, -maxFallingSpeed), _rigidbody.linearVelocity.z);
    }

    private void Init() { }


    public bool Jump()
    {
        Debug.Log("Jump");

        if (!_isGrounded || coyoteTimeCounter <= 0f)
            return false;

        Debug.Log("JumpEnter");

        _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
        _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        coyoteTimeCounter = 0f;

        return true;

    }

    private void CheckGrounded()
    {
        _isGrounded = Physics.Raycast(_rigidbody.position, Vector3.down, _halfHeight + _groundDetectionRange, groundLayer);
        Debug.Log("Is grounded :" + _isGrounded);
    }

    private void OnDrawGizmos()
    {
        if (_rigidbody == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_rigidbody.position, _rigidbody.position + Vector3.down * (_halfHeight + _groundDetectionRange));
    }

    private void HandleCoyoteTime(float delta)
    {
        if (_isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= delta;
    }
}
