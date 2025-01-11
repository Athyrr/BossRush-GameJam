using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "Game/Camera/Camera")]
public class PlayerCameraSO : ScriptableObject
{
    [Header("Speed")]
    [SerializeField]
    private float _rotationSpeed = 200;

    [SerializeField]
    private float _followSpeed = 200;

    [Header("Limits")]
    [SerializeField]
    private Vector2 _rotationLimits = new Vector2(-30, 45);

    [Header("Misc")]
    [SerializeField]
    [Tooltip("DOES NOT WORK YET")]
    private bool _inverseYaxe = false;


    public float RotationSpeed => _rotationSpeed;
    public float FollowSpeed => _followSpeed;
    public Vector2 RotationLimits => _rotationLimits;
    public bool inverseYaxe => _inverseYaxe;
}
