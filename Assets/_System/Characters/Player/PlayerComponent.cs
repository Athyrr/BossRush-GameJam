using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private float _groundDetectionRange = 0.2f;


    private Rigidbody _rigidbody = null;

    private float _halfHeight = 0;

    private bool _isGrounded = false;

    #endregion


    #region Lifecycle

    private void Awake()
    {
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();

        _halfHeight = GetComponent<Collider>().bounds.extents.y;
    }

    private void Update()
    {
        CheckGrounded();
    }

    #endregion


    #region Public API

    /// <summary>
    /// Is the player grounded ?
    /// </summary>
    public bool IsGrounded => _isGrounded;


    #endregion


    #region Private API

    /// <summary>
    /// Ground check.
    /// </summary>
    private void CheckGrounded()
    {
        _isGrounded = Physics.Raycast(_rigidbody.position, Vector3.down, _halfHeight + _groundDetectionRange, groundLayer);
        //Debug.Log("Is grounded :" + _isGrounded);
    }


    #endregion


    #region Debug

    private void OnDrawGizmos()
    {
        if (_rigidbody == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_rigidbody.position, _rigidbody.position + Vector3.down * (_halfHeight + _groundDetectionRange));
    }

    #endregion




}
