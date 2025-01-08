using UnityEngine;


/// <summary>
/// Informations about a moving entity.
/// </summary>
public struct MovementInfo
{
    /// <summary>
    /// The entity who is moving.
    /// </summary>
    public PlayerMovementComponent Mover;

    /// <summary>
    /// Movement speed.
    /// </summary>
    public float Speed;

    /// <summary>
    /// Track the movement direction. If it starts : target direction, it updates: current direction, if it ends: previous direction.
    /// </summary>
    public Vector3 Direction;

    /// <summary>
    /// Track the entity position.
    /// </summary>
    public Vector3 Position;

    /// <inheritdoc cref="MovementInfo"/>
    /// <param name="mover"></param>
    /// <param name="speed"></param>
    /// <param name="direction"></param>
    /// <param name="position"></param>
    public MovementInfo(PlayerMovementComponent mover, float speed, Vector3 direction, Vector3 position)
    {
        Mover = mover;
        Speed = speed;
        Direction = direction;
        Position = position;
    }
}
