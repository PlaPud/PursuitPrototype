using UnityEngine;

abstract public class IControllableOnGround : MonoBehaviour
{
    [Header("Ground Check")]
    [SerializeField] protected private LayerMask groundLayer;
    [SerializeField] protected private Vector2 boxSize;
    [SerializeField] protected private float castDistance;

    public bool IsGrounded { get; protected set; }

    void Start()
    {

    }

    void Update()
    {
        
    }

    virtual public void RayCheck() 
    {
        IsGrounded = Physics2D.BoxCast(
                            origin: transform.position, size: boxSize, angle: 0,
                            direction: -transform.up, distance: castDistance,
                            layerMask: groundLayer
                       );
    }
}
