using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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


    private PlayerComponent _player = null;
    private Rigidbody _rigidbody = null;

    private float coyoteTimeCounter;

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

        if (!TryGetComponent<PlayerComponent>(out _player))
        {
            Debug.LogError("player component not found!");
            return;
        }

        Init();
    }

    private void Update()
    {
        HandleCoyoteTime(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        HandleFalling();
    }

    private void HandleFalling()
    {
        if (_player.IsGrounded)
            return;

        _rigidbody.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
        _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, Mathf.Max(_rigidbody.linearVelocity.y, -maxFallingSpeed), _rigidbody.linearVelocity.z);
    }

    private void Init() { }


    public bool Jump()
    {
        Debug.Log("Jump");

        if (!_player.IsGrounded && coyoteTimeCounter <= 0f)
            return false;

        Debug.Log("JumpEnter");

        _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
        _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        coyoteTimeCounter = 0f;

        return true;

    }



    private void HandleCoyoteTime(float delta)
    {
        if (_player.IsGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= delta;
    }
}
