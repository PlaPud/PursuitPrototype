using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class EButtonFloating : MonoBehaviour
{
    [SerializeField] ControllingManager.Control overlayDisplayFor;
    [SerializeField] Vector2 overlayOffset = Vector2.zero;

    private Animator _overlayAnim;

    private const string EMPTY_BUTTON = "EmptyButton";
    private const string FILLUP_BUTTON = "FillUpButton";
    private const string FILLED_BUTTON = "FilledButton";
    private const string UNFILL_BUTTON = "UnfillButton";

    private string _currentAnim = EMPTY_BUTTON;


    public bool IsDisplayOnThisCtrl => ControllingManager.Instance.CurrentControl == overlayDisplayFor;

    private void Awake()
    {
        _overlayAnim = GetComponent<Animator>();    
    }

    void Start()
    {
        gameObject.SetActive(false);

        PlayerPushPull.OnAnyFoundMoveable += ShowOverlay;
        PlayerPushPull.OnAnyNotFoundMoveable += DisableOverlay;
        PlayerPushPull.OnAnyGrabbing += FillUnfillOverlay;

    }

    private void FillUnfillOverlay(bool isGrabbing, bool isInteract, float timer)
    {

        if (!isGrabbing) 
        {
            if (!isInteract)
            {
                if (_currentAnim == EMPTY_BUTTON) return;
                _currentAnim = EMPTY_BUTTON;
                return;
            }

            if (timer > 0f)
            {
                if (_currentAnim == FILLUP_BUTTON) return;
                _currentAnim = FILLUP_BUTTON;
                return;
            }

            return;
        }

        if (isGrabbing) 
        {
            if (!isInteract)
            {
                if (_currentAnim == FILLED_BUTTON) return;
                _currentAnim = FILLED_BUTTON;
                return;
            }

            if (timer > 0f)
            {
                if (_currentAnim == UNFILL_BUTTON) return;
                _currentAnim = UNFILL_BUTTON;
                return;
            }

            return;
        }
        
    }
   

    private void DisableOverlay(KeyCode interactKey, IControllableOnGround caster)
    {
        if (!gameObject.activeSelf) return;

        if (interactKey != KeyCode.E) return;

        _currentAnim = EMPTY_BUTTON;
        gameObject.SetActive(false);
    }

    private void ShowOverlay(KeyCode interactKey, Collider2D hitCD, IControllableOnGround caster)
    {
        if (interactKey != KeyCode.E) return;

        switch (overlayDisplayFor)
        {
            case ControllingManager.Control.PlayerMain:

                if (!IsDisplayOnThisCtrl)
                {
                    _currentAnim = EMPTY_BUTTON;
                    gameObject.SetActive(false);
                    return;
                }

                break;
            case ControllingManager.Control.CompBot:

                bool IsCtrlCompBotHitMoveable = ControllingManager.Instance.IsControllingCompBot
                    && ControllingManager.Instance.IsMatchedCompBot(
                            caster.GetComponent<CompBotController>()
                        );

                if (IsDisplayOnThisCtrl && !IsCtrlCompBotHitMoveable)
                {
                    _currentAnim = EMPTY_BUTTON;
                    gameObject.SetActive(false);
                    return;
                }
                break;
        }

        if (!IsDisplayOnThisCtrl) return;

        transform.position = hitCD.gameObject.transform.position + (Vector3)overlayOffset;

        if (gameObject.activeSelf) return;
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
        PlayerPushPull.OnAnyFoundMoveable -= ShowOverlay;
        PlayerPushPull.OnAnyNotFoundMoveable -= DisableOverlay;
        PlayerPushPull.OnAnyGrabbing -= FillUnfillOverlay;
    }

}
