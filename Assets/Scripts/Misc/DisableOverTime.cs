using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOverTime : MonoBehaviour
{
    [SerializeField] float lifeTime = 1f;

    private void OnEnable()
    {
        StartCoroutine(_DisableOverTime());
    }

    private IEnumerator _DisableOverTime() 
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);    
    }

}
