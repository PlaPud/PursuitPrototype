using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RButtonOverlay : MonoBehaviour
{
    [SerializeField] Vector2 overlayOffset = Vector2.zero;

    private Animator _overlayAnim;

    private const string EMPTY_BUTTON = "EmptyButton";
    private const string FILLUP_BUTTON = "FillUpButton";
    private const string FILLED_BUTTON = "FilledButton";
    private const string UNFILL_BUTTON = "UnfillButton";

    private string _currentAnim = EMPTY_BUTTON;
    private bool _isPlayAnim;

    public static Action<bool, float> OnFillUnfillOverlay;

    void Start()
    {
        _overlayAnim = GetComponent<Animator>();
        gameObject.SetActive(false);

        Interactable.OnAnyStayInteractable += ShowOverlay;
        Interactable.OnAnyExitInteractable += DisableOverlay;
        Interactable.OnAnyCountdownInteract += FillUnfillOverlay;
        Interactable.OnAnyInteraction += HandleOverlayAnim;
    }

    private void HandleOverlayAnim(bool isInteract, float timer, Interactable.Interact typeOfInteract, KeyCode interactKey)
    {
        if (!gameObject || interactKey != KeyCode.R) return;

        if (isInteract)
        {
            
        }
    }

    private void FillUnfillOverlay(bool isInteract, float timer) 
    {
        switch (ControllingManager.Instance.CurrentControl) 
        {
            case ControllingManager.Control.PlayerMain:

                if (!isInteract) 
                {
                    _currentAnim = EMPTY_BUTTON;
                    return;
                }

                if (timer > 0f) 
                {
                    _currentAnim = FILLUP_BUTTON;
                    return;
                }

                break;
            default:

                if (!isInteract)
                {
                    if (_currentAnim == FILLED_BUTTON) return;
                    _currentAnim = FILLED_BUTTON;
                    OnFillUnfillOverlay?.Invoke(isInteract, timer);
                    return;
                }

                if (timer > 0f)
                {
                    if (_currentAnim == UNFILL_BUTTON) return;
                    _currentAnim = UNFILL_BUTTON;
                    OnFillUnfillOverlay?.Invoke(isInteract, timer);
                    return;
                }
                break;
        }
    }

    private void DisableOverlay(KeyCode interactKey)
    {
        if (!gameObject.activeSelf) return;

        if (interactKey != KeyCode.R) return;

        gameObject.SetActive(false);
    }

    private void ShowOverlay(Transform destTransform, Collider2D collision, KeyCode interactKey)
    {
        if (interactKey != KeyCode.R) return;

        transform.position = destTransform.position + (Vector3) overlayOffset;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        _AnimStateMachine();
    }

    private void _AnimStateMachine()
    {
        switch (_currentAnim)
        {
            case EMPTY_BUTTON:
                _overlayAnim.Play(EMPTY_BUTTON);
                break;
            case FILLUP_BUTTON:
                _overlayAnim.Play(FILLUP_BUTTON);
                break;
            case UNFILL_BUTTON:
                _overlayAnim.Play(UNFILL_BUTTON);
                break;
            case FILLED_BUTTON:
                _overlayAnim.Play(FILLED_BUTTON);
                break;
        }
    }

    private void OnDestroy()
    {
        Interactable.OnAnyStayInteractable -= ShowOverlay;
        Interactable.OnAnyExitInteractable -= DisableOverlay;
        Interactable.OnAnyCountdownInteract -= FillUnfillOverlay;
        Interactable.OnAnyInteraction -= HandleOverlayAnim;
    }
}
