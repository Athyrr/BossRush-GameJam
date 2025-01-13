using UnityEngine;

[CreateAssetMenu(fileName = "JumpSO", menuName = "Game/Behaviors/Jump")]
public class EntityJumpSO : ScriptableObject
{
    [Header("Force")]
    [SerializeField]
    [Min(1f)]
    private float _jumpForce = 30f;

    [SerializeField]
    [Min(1f)]
    private float _gravityMultiplier = 60f;

    [Header("Misc")]
    [SerializeField]
    [Min(0f)]
    private float _coyoteTime = 0.2f;

    [SerializeField]
    [Min(0f)]
    private float _maxFallingSpeed = 50f;

    public float JumpForce => _jumpForce;
    public float GravityMultiplier => _gravityMultiplier;
    public float CoyoteTime => _coyoteTime;
    public float MaxFallingSpeed => Mathf.Max(_maxFallingSpeed, 1);
}
