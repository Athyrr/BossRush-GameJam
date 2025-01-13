using UnityEngine;

[CreateAssetMenu(fileName = "MovementSO", menuName = "Game/Behaviors/Movement")]
public class EntityMovementSO : ScriptableObject
{
    [Header("Speed")]
    [SerializeField]
    private float _speed = 0;

    [SerializeField]
    private float _maxSpeed = 0;


    [Header("Control")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float _airControl = 1f;

    [SerializeField]
    [Tooltip("Set to '1' means let the gravity moving the entity down.")]
    [Min(1.0f)]
    private float _fallingSpeedOnWall = 1f;


    [Header("Detection")]
    [SerializeField]
    [Tooltip("The layer mask with which the entity will collide.")]
    private LayerMask _collisionLayers = ~0;


    [Header("Debug")]
    [SerializeField]
    [Tooltip("Movement direction detection gizmo.")]
    private float _detectionRange = 0.2f;

    [SerializeField]
    [Tooltip("Movement direction detection gizmo.")]
    private Color _debugColor = Color.yellow;


    public float Speed => _speed;
    public float MaxSpeed => _maxSpeed;

    public float AirControl => Mathf.Clamp01(_airControl);
    public float FallingSpeedOnWall => _fallingSpeedOnWall;

    public LayerMask WallLayer => _collisionLayers;
    public float DetectionRange => _detectionRange;

    public Color DebugColor => _debugColor;

}
