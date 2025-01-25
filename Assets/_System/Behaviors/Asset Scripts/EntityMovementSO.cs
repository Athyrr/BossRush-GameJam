using UnityEngine;

[CreateAssetMenu(fileName = "MovementSO", menuName = "Game/Behaviors/Movement")]
public class EntityMovementSO : ScriptableObject
{
    [Header("Speed")]
    [SerializeField]
    private float _speed = 0;

    [SerializeField]
    private float _maxSpeed = 0;

    [SerializeField]
    private float _wallSlideSpeed = 0;

    [SerializeField]
    [Min(1)]
    private float _rotationSpeed = 0;

    [SerializeField]
    [Min(1)]
    private float _alignementSpeed = 0;


    [Header("Control")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float _airControl = 1f;

    [SerializeField]
    [Tooltip("Set to '1' means let the gravity moving the entity down.")]
    [Min(1.0f)]
    private float _fallingSpeedOnObstacle = 1f;

    [SerializeField]
    [Min(1.0f)]
    private float _acceleratonFactor = 1f;

    [SerializeField]
    [Min(1.0f)]
    private float _decelerationFactor = 1f;


    [Header("Slopes")]

    [SerializeField]
    [Range(0, 1)]
    private float _wallNormalThreshold = 0.5f;


    //[SerializeField]
    //[Tooltip("The max angle to considere that the player is on slope.")]
    //private float _maxSlopeAngle = 0f;

    //[SerializeField]
    //private float _slideSpeed = 0f;


    [Header("Detection")]
    [SerializeField]
    [Tooltip("The layer mask with which the entity will collide.")]
    private LayerMask _obstacleLayer = ~0;
    [SerializeField]
    [Tooltip("The layer mask of elements on which the entity can walk.")]
    private LayerMask _wallkableLayer = ~0;

    [SerializeField]
    private float _obstaclesDetectionRange = 0.2f;

    [SerializeField]
    private float _walkableDetectionRange = 0.2f;

    [SerializeField]
    private float _wallDetectionOffset = 0.2f;


    [Header("Debug")]
    [SerializeField]
    private Color _movementDirectionColor = Color.yellow;

    [SerializeField]
    private Color _walkableDetectionColor = Color.cyan;

    [SerializeField]
    private Color _obstacleDetectionColor = Color.green;


    public float Speed => _speed;
    public float MaxSpeed => _maxSpeed;
    public float WallSlideSpeed => _wallSlideSpeed;
    public float AlignementSpeed => _alignementSpeed;
    public float RotationSpeed => _rotationSpeed;

    public float AirControl => Mathf.Clamp01(_airControl);
    public float FallingSpeedOnObstacles => _fallingSpeedOnObstacle;
    public float AcceleratonFactor => _acceleratonFactor;
    public float DecelerationFactor => _decelerationFactor;
    public float WallNormalThreshold => _wallNormalThreshold;

    public LayerMask WalkableLayer => _wallkableLayer;
    public LayerMask ObstacleLayer => _obstacleLayer;
    public float ObstacleDetectionRange => _obstaclesDetectionRange;
    public float WalkableDetectionRange => _walkableDetectionRange;
    public float ForwardDetectionOffset => _wallDetectionOffset;

    public Color MovementDirectionColor => _movementDirectionColor;
    public Color WalkableDetectionColor => _walkableDetectionColor;
    public Color ObstacleDetectionColor => _obstacleDetectionColor;



}
