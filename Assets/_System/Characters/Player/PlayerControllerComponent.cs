using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerComponent : MonoBehaviour
{
    #region Fields

    private GameInputs _gameInputs = null;

    [SerializeField]
    private PlayerCameraComponent _camera = null;

    private PlayerMovementComponent _movement = null;
    private PlayerJumpComponent _jump = null;
    private PlayerDashComponent _dash = null;

    private Vector3 _movementDirection = Vector3.zero;
    private Vector3 _previousMovementDirection;

    private Vector2 _cameraLookInput;

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

        //Camera cam = GetComponentInChildren<Camera>();
        //if (cam == null)
        //    Debug.LogError($"{nameof(Camera)} component not found in child", this);

        //_camera = cam.GetComponent<PlayerCameraComponent>();
        //if (_camera == null)
        //    Debug.LogError($"{nameof(PlayerCameraComponent)} component not found in camera", this);
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
        float delta = Time.fixedDeltaTime;
        UpdateMovement(delta);
    }

    private void LateUpdate()
    {

        float delta = Time.deltaTime;
        UpdateCameraLook(delta);
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

        _gameInputs.Game.Look.performed += HandleLookInput;
        _gameInputs.Game.Look.canceled += HandleLookInput;
    }

    private void UnBindInputs()
    {
        _gameInputs.Game.Move.performed -= HandleMoveInput;
        _gameInputs.Game.Move.canceled -= HandleMoveInput;

        _gameInputs.Game.Jump.started -= HandleJumpInput;

        _gameInputs.Game.Dash.started -= HandleDashInput;

        _gameInputs.Game.Sprint.started -= HandleSprintInput;
        _gameInputs.Game.Sprint.canceled -= HandleSprintInput;

        _gameInputs.Game.Look.performed -= HandleLookInput;
        _gameInputs.Game.Look.canceled -= HandleLookInput;
    }
    private void UpdateMovement(float delta)
    {
        if (_movementDirection == Vector3.zero)
            return;

        Vector3 cameraForward = _camera.transform.forward;
        Vector3 cameraRight = _camera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;


        Vector3 movement3D = cameraForward * _movementDirection.y + cameraRight * _movementDirection.x;
        movement3D.Normalize();

        //@todo in movement component
        if (movement3D != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement3D), delta * 10f);

        _previousMovementDirection = movement3D;

        _movement.Move(movement3D, delta);
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
        //Vector3 dashDirection = _movementDirection == Vector3.zero ? _previousMovementDirection : _movementDirection; 
        //_dash.Dash(_movementDirection);
        _dash.Dash(transform.forward);
    }
    private void HandleLookInput(InputAction.CallbackContext context)
    {
        _cameraLookInput = context.ReadValue<Vector2>();
    }

    private void UpdateCameraLook(float delta)
    {
        _cameraLookInput.Normalize();

        _camera.Look(_cameraLookInput, delta);
    }

    #endregion
}