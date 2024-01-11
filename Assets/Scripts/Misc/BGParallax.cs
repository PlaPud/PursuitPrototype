using Cinemachine;
using Codice.Client.BaseCommands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGParallax : MonoBehaviour
{
    [SerializeField] private float parallaxSpeed = 0.02f;

    private Vector3 _initialPosition;
    private PlayerController _playerCat;

    private CinemachineBrain _mainCamBrain;

    void Start()
    {
        _mainCamBrain = Camera.main.GetComponent<CinemachineBrain>();
        _playerCat = ControllingManager.Instance.CatController;
        _initialPosition = transform.position;
    }

    void Update()
    {
        if (_mainCamBrain && _mainCamBrain.ActiveVirtualCamera.VirtualCameraGameObject.CompareTag("FixedCam")) return;
        MoveParallax();
    }

    public void MoveParallax()
    {
        float parallax = (_playerCat.transform.position.x - _initialPosition.x) * parallaxSpeed;
        Vector3 newPosition = _initialPosition + new Vector3(parallax, 0, 0);
        transform.position = newPosition;
    }
}
