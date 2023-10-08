using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawWireController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wirePoint;

    private LineRenderer _wireLR;
    private RaycastHit2D _hitCeiling;
    

    private void Awake()
    {
        _wireLR = GetComponent<LineRenderer>();    
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    

    private void CheckRayCeiling()
    {
        _hitCeiling = Physics2D.Raycast(
                        transform.position,
                        Vector2.up,
                        groundLayer
                    );
    }
}
