using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VCamFixed : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera mainVCam;

    [SerializeField] private CinemachineVirtualCamera currentVCam;

    private void Awake()
    {
        //currentVCam = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("PlayerCat")) return;
        currentVCam.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("PlayerCat")) return;
        currentVCam.gameObject.SetActive(false);
    }
}
