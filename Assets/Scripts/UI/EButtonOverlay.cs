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

    void Start()
    {
        _overlayAnim = GetComponent<Animator>();    
        gameObject.SetActive(false);
        Interactable.OnStayInteractable += ShowOverlay;
        Interactable.OnExitInteractable += DisableOverlay;
        Interactable.OnInteraction += HandleOverlayState;
    }

    private void Update()
    {
        if (!onGroundPlayer) return;
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

    private void ShowOverlay() 
    {
        gameObject.SetActive(true);        
    }

    private void DisableOverlay() 
    {
        gameObject.SetActive(false);
    }

    private void HandleOverlayState(bool isInteract, float timer, Interactable.Interact typeOfInteract) 
    {
        switch(typeOfInteract) 
        {
            case Interactable.Interact.PressToInteract:
                _HandlePressAnim(isInteract, timer);
                break;
            case Interactable.Interact.HoldToInteract:

                break;
        }
    }

    private void _HandlePressAnim(bool isInteract, float timer) 
    {
        if (!isInteract)
        {
            if (_currentAnim != EMPTY_BUTTON)
            {
                _currentAnim = EMPTY_BUTTON;
                _overlayAnim.Play(EMPTY_BUTTON);
                return;
            }
        }
        else
        {
            if (_currentAnim != FILLED_BUTTON)
            {
                _currentAnim = FILLED_BUTTON;
                _overlayAnim.Play(FILLED_BUTTON);
                return;
            }
        }
    }

}
