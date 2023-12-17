using System.Collections;
using UnityEngine;

public class EButtonOverlay : MonoBehaviour
{
    [SerializeField] ControllingManager.Control overlayDisplayFor;
    [SerializeField] Vector2 overlayOffset = Vector2.zero;

    private Animator _overlayAnim;

    private const string EMPTY_BUTTON = "EmptyButton";
    private const string FILLED_BUTTON = "FilledButton";

    private string _currentAnim = EMPTY_BUTTON;
    private bool _isPlayingAnim;

    public bool IsDisplayOnThisCtrl => ControllingManager.Instance.CurrentControl == overlayDisplayFor;

    void Start()
    {
        _overlayAnim = GetComponent<Animator>();
        gameObject.SetActive(false);

        Interactable.OnAnyStayInteractable += ShowOverlay;
        Interactable.OnAnyExitInteractable += DisableOverlay;
        Interactable.OnAnyInteraction += HandleOverlayAnim;
    }

    void Update()
    {

        if (!IsDisplayOnThisCtrl || !gameObject.activeSelf) return;

        _AnimStateMachine();
    }

    private void DisableOverlay(KeyCode interactKey)
    {
        if (!gameObject.activeSelf) return;

        if (interactKey != KeyCode.E) return;

        _currentAnim = EMPTY_BUTTON;
        _isPlayingAnim = false;
        gameObject.SetActive(false);
    }

    private void ShowOverlay(Transform destTransform, Collider2D collision, KeyCode interactKey)
    {
        if (interactKey != KeyCode.E) return;

        bool IsCtrlCompBotInCD = ControllingManager.Instance.IsControllingCompBot
            && ControllingManager.Instance
                .IsMatchedCompBot(
                    collision.GetComponent<CompBotController>()
                );

        switch (overlayDisplayFor) 
        {
            case ControllingManager.Control.PlayerMain:
                if (!IsDisplayOnThisCtrl) 
                {
                    _currentAnim = EMPTY_BUTTON;
                    _isPlayingAnim = false;
                    gameObject.SetActive(false);
                    return;
                }
                break;
            case ControllingManager.Control.CompBot:
                if (IsDisplayOnThisCtrl && !IsCtrlCompBotInCD) 
                {
                    _currentAnim = EMPTY_BUTTON;
                    _isPlayingAnim = false;
                    gameObject.SetActive(false);
                    return;
                }
                break;
        }

        if (!IsDisplayOnThisCtrl) return;

        if (gameObject.activeSelf) return;

        transform.position = destTransform.position + (Vector3) overlayOffset;
        gameObject.SetActive(true);
    }

    private void HandleOverlayAnim(bool isInteract, float timer, Interactable.Interact typeOfInteract, KeyCode interactKey)
    {
        if (interactKey != KeyCode.E) return;

        if (!IsDisplayOnThisCtrl) return;

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
            if (_isPlayingAnim || !gameObject.activeSelf) return;

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

    private void OnDestroy()
    {
        Interactable.OnAnyStayInteractable -= ShowOverlay;
        Interactable.OnAnyExitInteractable -= DisableOverlay;
        Interactable.OnAnyInteraction -= HandleOverlayAnim;
    }

}
