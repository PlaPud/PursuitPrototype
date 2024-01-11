using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObj : MonoBehaviour
{

    [SerializeField] private float flipTime = 0.5f;
    [SerializeField] private float smoothTime = 0.05f;
    private bool _isCatFacingRight;

    private PlayerController _playerCat;

    private bool _isDoneFlip;

    private Vector3 velocity;

    void Start()
    {
        ControllingManager.Instance.CatController.OnPlayerFlipped += _FlipCamLerp;  
        _playerCat = ControllingManager.Instance.CatController; 
    }

    void FixedUpdate()
    {
        //transform.position = _playerCat.transform.position;
        transform.position = Vector3.SmoothDamp(
                transform.position,
                _playerCat.transform.position, 
                ref velocity, 
                smoothTime
            );
    }

    private void _FlipCamLerp()
    {
        StartCoroutine(_FlipCamLerpCoroutine());
    }

    private IEnumerator _FlipCamLerpCoroutine()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = _GetNextRotation();

        float yRotation = 0f;
        float timeElapsed = 0f;

        while (timeElapsed < flipTime)
        {
            timeElapsed += Time.deltaTime;
            yRotation = Mathf.Lerp(startRotation, endRotationAmount, timeElapsed / flipTime);
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

            if (timeElapsed >= flipTime) break;

            yield return null;
        }

    }

    private float _GetNextRotation()
    {
        _isCatFacingRight = !_isCatFacingRight;
        return _isCatFacingRight ? 180f : 0f;
    }

    private void OnDestroy()
    {
        ControllingManager.Instance.CatController.OnPlayerFlipped -= _FlipCamLerp;
    }
}
