using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RButtonMarkerUI : MonoBehaviour
{
    private const float ARROW_IMG_ANGLE_OFFSET = -90f;

    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector2 borderSize;

    private RectTransform arrowTransform;
    private RectTransform buttonTransform;

    public static Action OnOverlayOffScreen;

    private float _angle;
    private string _currentAnim;

    private const string FILLED_BUTTON = "FilledButtonUI";
    private const string UNFILL_BUTTON = "UnfillButtonUI";

    private Animator _btnAnim;

    private Vector3 _targetScreenPos => Camera.main.WorldToScreenPoint(targetTransform.position);
    private bool _offScreenLeft => _targetScreenPos.x <= borderSize.x;
    private bool _offScreenRight => _targetScreenPos.x >= Screen.width - borderSize.x;
    private bool _offScreenDown => _targetScreenPos.y <= borderSize.y;
    private bool _offScreenUp => _targetScreenPos.y >= Screen.height - borderSize.y;

    private bool _isMarkerNotActive => !arrowTransform.gameObject.activeSelf || !buttonTransform.gameObject.activeSelf;

    private void Awake()
    {

        arrowTransform = GetComponentsInChildren<RectTransform>()[1];
        buttonTransform = GetComponentsInChildren<RectTransform>()[2];

        _btnAnim = buttonTransform.GetComponent<Animator>();
    }

    void Start()
    {
        arrowTransform.gameObject.SetActive(false);
        buttonTransform.gameObject.SetActive(false);

        RButtonOverlay.OnFillUnfillOverlay += HandleFillUnfill;
    }

    void Update()
    {
        if (!targetTransform.gameObject.activeSelf) 
        {
            arrowTransform.gameObject.SetActive(false);
            buttonTransform.gameObject.SetActive(false);
            return;
        }

        RotateMarker();
        HandleMarkerOffScreen();
    }

    private void HandleFillUnfill(bool isInteract, float timer) 
    {
        if (_isMarkerNotActive) return;

        if (!isInteract)
        {
            _currentAnim = FILLED_BUTTON;
            _btnAnim.Play(_currentAnim);
            return;
        }

        if (timer > 0f)
        {
            _currentAnim = UNFILL_BUTTON;
            _btnAnim.Play(_currentAnim);
            return;
        }
    }

    private void RotateMarker()
    {
        if (_isMarkerNotActive) return;

        Vector3 toPos = targetTransform.position;
        Vector3 fromPos = Camera.main.transform.position;
        fromPos.z = 0f;
        _angle = _GetAngle(toPos, fromPos);

        arrowTransform.localEulerAngles = new Vector3(0, 0, _angle + ARROW_IMG_ANGLE_OFFSET);
    }

    private float _GetAngle(Vector3 toPos, Vector3 fromPos)
    {
        float angle;
        Vector3 targetDir = (toPos - fromPos).normalized;
        angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        return angle;
    }

    private void HandleMarkerOffScreen()
    {
        bool isOffScreen = _offScreenLeft || _offScreenRight || _offScreenUp || _offScreenDown;

        if (!isOffScreen)
        {
            arrowTransform.gameObject.SetActive(false);
            buttonTransform.gameObject.SetActive(false);
            return;
        }

        _EnableMarker();
        _PositioningArrow();
        _PositioningKeyOnArrow();

        OnOverlayOffScreen?.Invoke();
    }

    private void _PositioningKeyOnArrow()
    {
        buttonTransform.position = arrowTransform.position - 60f * new Vector3(
                        Mathf.Cos(Mathf.Deg2Rad * _angle),
                        Mathf.Sin(Mathf.Deg2Rad * _angle),
                        0f
                    );
    }

    private void _PositioningArrow()
    {
        Vector3 marginScreenPos = _targetScreenPos;

        if (_offScreenLeft) marginScreenPos.x = borderSize.x;
        if (_offScreenRight) marginScreenPos.x = Screen.width - borderSize.x;
        if (_offScreenDown) marginScreenPos.y = borderSize.y;
        if (_offScreenUp) marginScreenPos.y = Screen.height - borderSize.y;

        arrowTransform.position = marginScreenPos;
    }

    private void _EnableMarker()
    {
        if (_isMarkerNotActive)
        {
            arrowTransform.gameObject.SetActive(true);
            buttonTransform.gameObject.SetActive(true);
        }
    }
}
