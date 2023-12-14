using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EButtonOverlay : MonoBehaviour
{
    [SerializeField] private float xLocalOffset;
    [SerializeField] private float yLocalOffset;

    [SerializeField] IControllableOnGround onGroundPlayer;

    private Animator _overlayAnim;

    private const string EMPTY_BUTTON = "EmptyButton";
    private const string FILLUP_BUTTON = "FillUpButton";
    private const string FILLED_BUTTON = "FilledButton";
    private const string UNFILL_BUTTON = "UnfillButton";

    private string _currentAnim = EMPTY_BUTTON;
    private bool _isPlayingAnim;
    
    public bool IsDisplayOnThis => 
        ControllingManager.Instance.IsControllingCat && onGroundPlayer is PlayerController
            ||
        ControllingManager.Instance.IsControllingCompBot && onGroundPlayer is CompBotController;

    void Start()
    {
        _overlayAnim = GetComponent<Animator>();
        gameObject.SetActive(false);
        Interactable.OnAnyStayInteractable += ShowOverlay;
        Interactable.OnAnyExitInteractable += DisableOverlay;
        Interactable.OnAnyInteraction += HandleOverlayAnim;
    }

    private void Update()
    {
        if (!onGroundPlayer) return;

        if (!IsDisplayOnThis) return;

        _AnimStateMachine();
    }

    private void LateUpdate()
    {
        if (!onGroundPlayer) return;

        transform.position =
            onGroundPlayer.transform.position
            + new Vector3(
                    xLocalOffset * Mathf.Sign(onGroundPlayer.transform.localScale.x),
                    yLocalOffset,
                    0f
                );
    }

    private void ShowOverlay(KeyCode interactKey)
    {
        if (interactKey != KeyCode.E) return;

        if (!IsDisplayOnThis)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
    }

    private void DisableOverlay(KeyCode interactKey)
    {
        if (interactKey != KeyCode.E) return;

        if (!gameObject.activeSelf) return;
        gameObject.SetActive(false);
    }

    private void HandleOverlayAnim(bool isInteract, float timer, Interactable.Interact typeOfInteract, KeyCode interactKey)
    {
        if (interactKey != KeyCode.E) return;

        if (!IsDisplayOnThis) return;

        switch (typeOfInteract)
        {
            case Interactable.Interact.PressToInteract:
                _HandlePressAnim(isInteract, timer);
                break;
            case Interactable.Interact.HoldToInteract:
                _HandleHoldAnim(isInteract, timer);
                break;
        }
    }

    private void _HandlePressAnim(bool isInteract, float timer)
    {
        if (!isInteract)
        {
            if (_isPlayingAnim) return;

            if (_currentAnim != EMPTY_BUTTON)
            {
                _currentAnim = EMPTY_BUTTON;
                return;
            }
        }
        else
        {
            if (_isPlayingAnim) return;

            if (_currentAnim != FILLED_BUTTON)
            {
                StartCoroutine(_PlayPressedAnim());
            }
        }
    }

    private IEnumerator _PlayPressedAnim()
    {
        _isPlayingAnim = true;
        _currentAnim = FILLED_BUTTON;
        yield return new WaitForSeconds(.25f);
        _currentAnim = EMPTY_BUTTON;
        _isPlayingAnim = false;
    }

    private void _AnimStateMachine() 
    {
        switch (_currentAnim) 
        {
            case EMPTY_BUTTON:
                _overlayAnim.Play(EMPTY_BUTTON);
                break;
            case FILLED_BUTTON:
                _overlayAnim.Play(FILLED_BUTTON);
                break;
        }
    }

    private void _HandleHoldAnim(bool isInteract, float timer) 
    {
        
    }

}
