using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
    private ShootComponent _shoot = null;
    private WallRunComponent _wallRun = null;

    private Vector3 _movementDirection = Vector3.zero;
    private Vector3 _previousMovementDirection = Vector3.zero;
    private Vector2 _cameraLookInput = Vector2.zero;

    private bool _isHoldingWallRunInput = false;
    private bool _isAiming = false;

    private Ray _aimTargetRay;

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

        if (!TryGetComponent<ShootComponent>(out _shoot))
            Debug.LogError($"{nameof(ShootComponent)} component not found", this);

        if (!TryGetComponent<WallRunComponent>(out _wallRun))
            Debug.LogError($"{nameof(WallRunComponent)} component not found", this);

        if (FindFirstObjectByType<PlayerCameraComponent>() == null)
            Debug.LogError($"{nameof(PlayerCameraComponent)} component not found", this);
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

    private void Update()
    {
        if (_isAiming) 
            UpdateAiming();
    }

    void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        UpdateMovement(delta);

        if (_isHoldingWallRunInput)
            PerfomWallRun();
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

        _gameInputs.Game.Look.performed += HandleLookInput;
        _gameInputs.Game.Look.canceled += HandleLookInput;

        _gameInputs.Game.WallRun.performed += HandleWallRunInput;
        _gameInputs.Game.WallRun.canceled += HandleWallRunInput;

        _gameInputs.Game.Aim.performed += HandleAimInput;
        _gameInputs.Game.Aim.canceled += HandleAimInput;

        _gameInputs.Game.Shoot.performed += HandleShootInput;
        //_gameInputs.Game.Shoot.canceled += HandleShootInput;
    }

    private void UnBindInputs()
    {
        _gameInputs.Game.Move.performed -= HandleMoveInput;
        _gameInputs.Game.Move.canceled -= HandleMoveInput;

        _gameInputs.Game.Jump.started -= HandleJumpInput;

        _gameInputs.Game.Dash.started -= HandleDashInput;

        _gameInputs.Game.Look.performed -= HandleLookInput;
        _gameInputs.Game.Look.canceled -= HandleLookInput;

        _gameInputs.Game.WallRun.performed -= HandleWallRunInput;
        _gameInputs.Game.WallRun.canceled -= HandleWallRunInput;

        _gameInputs.Game.Aim.performed -= HandleAimInput;
        _gameInputs.Game.Aim.canceled -= HandleAimInput;

        _gameInputs.Game.Shoot.performed -= HandleShootInput;
        //_gameInputs.Game.Shoot.canceled -= HandleShootInput;
    }

    private void UpdateMovement(float delta)
    {
        Vector3 cameraForward = _camera.transform.forward;
        Vector3 cameraRight = _camera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        Vector3 movement3D = cameraForward * _movementDirection.y + cameraRight * _movementDirection.x;
        movement3D.Normalize();

        _movement.Move(movement3D, delta);

        _previousMovementDirection = movement3D;
    }


    private void HandleMoveInput(InputAction.CallbackContext context)
    {
        _movementDirection = context.ReadValue<Vector2>();
    }

    private void HandleJumpInput(InputAction.CallbackContext context)
    {
        _jump.Jump();
    }

    private void HandleDashInput(InputAction.CallbackContext context)
    {
        if (!_dash.Settings.AllowDashWhileStanding && _movementDirection == Vector3.zero)
            return;

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

    private void HandleAimInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isAiming = true;
            Debug.Log("Aim");
        }
        else if (context.canceled)
        {
            _isAiming = false;
            Debug.Log("Release Aim");
        }
    }

    private void UpdateAiming()
    {
        if (!_isAiming) return;

        Vector3 crossHair = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        _aimTargetRay = Camera.main.ScreenPointToRay(crossHair);
    }

    private void HandleShootInput(InputAction.CallbackContext context)
    {
        if (!_isAiming)
            return;

        _shoot.Shoot(_aimTargetRay);
    }

    private void HandleWallRunInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isHoldingWallRunInput = true;
        }
        else if (context.canceled)
        {
            _isHoldingWallRunInput = false;
            _wallRun.StopWallRun();
        }
    }

    private void PerfomWallRun()
    {
        _wallRun.WallRun();
    }

    #endregion
}