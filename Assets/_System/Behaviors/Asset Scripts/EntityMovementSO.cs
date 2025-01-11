using UnityEngine;

[CreateAssetMenu(fileName = "MovementSO", menuName = "Game/Behaviors/Movement")]
public class EntityMovementSO : ScriptableObject
{
    [SerializeField]
    private float _speed = 0;

    [SerializeField]
    private float _maxSpeed = 0;

    [Min(0)]
    [SerializeField]
    private float _smoothness = 0;

    public float Speed => _speed;
    public float MaxSpeed => _maxSpeed;
    public float Smoothness => Mathf.Max(_smoothness, 1);
}
