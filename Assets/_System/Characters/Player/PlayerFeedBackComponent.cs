using Unity.VisualScripting;
using UnityEngine;

public class PlayerFeedbackComponent : MonoBehaviour
{


    [SerializeField]
    private Animator animator;

    private PlayerMovementComponent movementComponent;
    private PlayerDashComponent dashComponent;
    private PlayerJumpComponent jumpComponent;


    private void Awake()
    {
        TryGetComponent<PlayerMovementComponent>(out movementComponent);
        TryGetComponent<PlayerDashComponent>(out dashComponent);
        TryGetComponent<PlayerJumpComponent>(out jumpComponent);
    }

    private void OnEnable()
    {
        //movementComponent.OnMoveStart.AddListener(HandleMoveStart);
        movementComponent.OnMoveUpdate.AddListener(HandleMoveUpdate);
        movementComponent.OnMoveEnd.AddListener(HandleMoveEnd);

        dashComponent.OnDashStart.AddListener(HandleDashStart);

        jumpComponent.OnJumpStart.AddListener(HandleJumpStart);
    }

    private void OnDisable()
    {
        //movementComponent.OnMoveStart.RemoveListener(HandleMoveStart);
        movementComponent.OnMoveUpdate.RemoveListener(HandleMoveUpdate);
        movementComponent.OnMoveEnd.RemoveListener(HandleMoveEnd);

        dashComponent.OnDashStart.RemoveListener(HandleDashStart);

        jumpComponent.OnJumpStart.RemoveListener(HandleJumpStart);
    }

    /*private void HandleMoveStart(MovementInfo movementInfo)
    {
        
    }*/

    private void HandleMoveUpdate(MovementInfo movementInfo)
    {
        animator.SetFloat("Speed", movementInfo.Speed);
    }

     private void HandleMoveEnd(MovementInfo movementInfo)
     {
        animator.SetFloat("Speed", 0);
     }

    private void HandleDashStart()
    {
        animator.SetTrigger("isDashing");
    }

    private void HandleJumpStart()
    {
        animator.SetTrigger("isJumping");
    }
}
