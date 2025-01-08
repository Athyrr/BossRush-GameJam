using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerComponent : MonoBehaviour
{
    #region Fields

    private GameInputs _gameInputs = null;

    private PlayerMovementComponent _movement = null;
    private PlayerJumpComponent _jump = null;
    private PlayerDashComponent _dash = null;

    private Vector3 _movementDirection = Vector3.zero;
    private Vector3 _previousMovementDirection = Vector3.zero;

    private float _delta = 0f;

    #endregion


    #region Lifecycle

    private void Awake()
    {
        if (!TryGetComponent<PlayerMovementComponent>(out _movement))
            Debug.LogError($"{nameof(PlayerMovementComponent)} component not found", this);

        if (!TryGetComponent<PlayerJumpComponent>(out _jump))
            Debug.LogError($"{nameof(PlayerJumpComponent)} component not found", this);

        if (!TryGetComponent<PlayerDashComponent>(out _dash))
            Debug.LogError($"{nameof(PlayerDashComponent)} component not found", this);
    }


    private void OnEnable()
    {
        if (_gameInputs == null)
        {
            _gameInputs = new GameInputs();
            BindInputs();
        }

        _gameInputs.Enable();
    }

    private void OnDisable()
    {
        UnBindInputs();
        _gameInputs.Disable();
    }

    void FixedUpdate()
    {
        _delta = Time.fixedDeltaTime;
        UpdateMovement(_delta);
    }



    #endregion

    #region Private API

    private void BindInputs()
    {
        _gameInputs.Game.Move.performed += HandleMoveInput;
        _gameInputs.Game.Move.canceled += HandleMoveInput;

        _gameInputs.Game.Jump.started += HandleJumpInput;

        _gameInputs.Game.Dash.started += HandleDashInput;

        _gameInputs.Game.Sprint.started += HandleSprintInput;
        _gameInputs.Game.Sprint.canceled += HandleSprintInput;
    }

    private void UnBindInputs()
    {
        _gameInputs.Game.Move.performed -= HandleMoveInput;
        _gameInputs.Game.Move.canceled -= HandleMoveInput;

        _gameInputs.Game.Jump.started -= HandleJumpInput;

        _gameInputs.Game.Dash.started -= HandleDashInput;

        _gameInputs.Game.Sprint.started -= HandleSprintInput;
        _gameInputs.Game.Sprint.canceled -= HandleSprintInput;
    }

    private void UpdateMovement(float delta)
    {
        if (_movementDirection == Vector3.zero) return;

        _movementDirection.Normalize();

        Vector3 movement3D = new Vector3(_movementDirection.x, 0, _movementDirection.y);
        _movement.Move(movement3D, delta);

        _previousMovementDirection = _movementDirection;
    }

    private void HandleMoveInput(InputAction.CallbackContext context)
    {
        _movementDirection = context.ReadValue<Vector2>();
    }

    private void HandleJumpInput(InputAction.CallbackContext context)
    {
        _jump.Jump();
    }

    private void HandleSprintInput(InputAction.CallbackContext context)
    {
        Debug.Log("Sprint");
    }

    private void HandleDashInput(InputAction.CallbackContext context)
    {
        _dash.Dash();
    }

    #endregion
}