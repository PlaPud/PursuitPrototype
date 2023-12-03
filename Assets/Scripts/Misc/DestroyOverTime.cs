using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{
    [SerializeField] float lifeTime = 1f;

    private Transform _effectParent;

    void Start()
    {
        Destroy(gameObject, lifeTime);   
    }

}
