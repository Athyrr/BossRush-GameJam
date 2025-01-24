using UnityEngine;

public class PlayerJumpComponent : MonoBehaviour
{

    #region Fields

    [SerializeField]
    private EntityJumpSO _settings = null;


    private Rigidbody _rigidbody = null;
    private PlayerComponent _player = null;

    private float _coyoteTimeCounter;

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

    private void Init() { }

    #endregion


    #region Public API

    public bool Jump()
    {
      //  if (!_player.IsGrounded && _coyoteTimeCounter <= 0f)
            return false;

        _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
        _rigidbody.AddForce(Vector3.up * _settings.JumpForce, ForceMode.Impulse);
        _coyoteTimeCounter = 0f;

        return true;

    }

    #endregion


    #region Private API

    private void HandleFalling()
    {
        //if (_player.IsGrounded)
            return;

        _rigidbody.AddForce(Vector3.down * _settings.GravityMultiplier, ForceMode.Acceleration);
        _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, Mathf.Max(_rigidbody.linearVelocity.y, -_settings.MaxFallingSpeed), _rigidbody.linearVelocity.z);
    }

    private void HandleCoyoteTime(float delta)
    {
        //if (_player.IsGrounded)
            _coyoteTimeCounter = _settings.CoyoteTime;
       // else
            _coyoteTimeCounter -= delta;
    }

    #endregion

}
