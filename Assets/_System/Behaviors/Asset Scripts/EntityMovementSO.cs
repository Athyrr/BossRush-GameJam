using UnityEngine;

[CreateAssetMenu(fileName = "MovementSO", menuName = "Game/Behaviors/Movement")]
public class EntityMovementSO : ScriptableObject
{
    [Header("Speed")]
    [SerializeField]
    private float _speed = 0;

    [SerializeField]
    private float _maxSpeed = 0;

    [Header("Smoothness")]
    [Min(0)]
    [SerializeField]
    private float _smoothness = 0;

    [Header("Detection")]
    [SerializeField]
    [Tooltip("The walls layer mask.")]
    private LayerMask _wallLayer = ~0;

    [SerializeField]
    [Tooltip("The NMEs layer mask.")]
    private LayerMask _enemiesLayer = ~0;

    [Header("Debug")]
    [SerializeField]
    [Tooltip("Movement direction detection gizmo.")]
    private float _detectionRange = 0.2f;

    [SerializeField]
    [Tooltip("Movement direction detection gizmo.")]
    private Color _debugColor = Color.yellow;


    public float Speed => _speed;
    public float MaxSpeed => _maxSpeed;
    public float Smoothness => Mathf.Max(_smoothness, 1);
    public LayerMask WallLayer => _wallLayer;
    public LayerMask EnemyLayer => _enemiesLayer;
    public float DetectionRange => _detectionRange;
    public Color DebugColor => _debugColor;

}
