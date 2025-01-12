using UnityEngine;

[CreateAssetMenu(fileName = "DashSO", menuName = "Game/Behaviors/Dash")]
public class EntityDashSO : ScriptableObject
{
    [Header("Force")]
    [SerializeField]
    [Tooltip("Force of the dash. More force is, farther player will ends.")]
    [Min(1)]
    private float _dashForce = 0f;

    [Header("Time")]
    [SerializeField]
    [Min(0)]
    [Tooltip("Cooldown duration before be able to dash again.")]
    private float _coolddown = 0f;

    [SerializeField]
    [Tooltip("Dash duration.")]
    [Min(0)]
    private float _duration = 0f;

    [Header("Momemtum")]
    [SerializeField]
    [Tooltip("The dash momentum depending on its duration. X axis: Time ratio between 0 and 1. Y axis: The dash postition based on its estimatated end dash position.")]
    private AnimationCurve _dashMomentum = null;

    [SerializeField]
    [Tooltip("Allow dash while standing.")]
    private bool _allowDashWhileStanding = false;

    [SerializeField]
    [Tooltip("Allow dash in the air. DOES NOT WORK YET")]
    private bool _allowDashInAir = false;

    [Header("Detection")]
    [SerializeField]
    [Tooltip("The walls layer mask.")]
    private LayerMask _wallLayer = ~0;

    [SerializeField]
    [Tooltip("The NMEs layer mask.")]
    private LayerMask _enemiesLayer = ~0;

    [Header("Debug")]
    [SerializeField]
    [Tooltip("Gizmo color.")]
    private Color _debugColor = Color.green;

    public float DashForce => Mathf.Max(1, _dashForce);
    public float Coolddown => Mathf.Max(0, _coolddown);
    public float Duration => Mathf.Max(0.1f, _duration);
    public AnimationCurve Momentum => _dashMomentum;
    public bool AllowDashWhileStanding => _allowDashWhileStanding;
    public bool AllowDashInAir => _allowDashInAir;
    public LayerMask WallLayer => _wallLayer;
    public LayerMask EnemyLayer => _enemiesLayer;
    public Color DebugColor => _debugColor; 
}
