using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RButtonOverlay : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
        Interactable.OnAnyStayInteractable += ShowOverlay;
        Interactable.OnAnyExitInteractable += DisableOverlay;
        Interactable.OnAnyInteraction += HandleOverlayAnim;
    }

    private void HandleOverlayAnim(bool isInteract, float timer, Interactable.Interact typeOfInteract, KeyCode interactKey)
    {
    }

    private void DisableOverlay(KeyCode interactKey)
    {
        if (interactKey != KeyCode.R) return;

        gameObject.SetActive(false);
        return;
    }

    private void ShowOverlay(KeyCode interactKey)
    {
        if (interactKey != KeyCode.R) return;

        gameObject.SetActive(true);
    }

    void Update()
    {
        
    }
}
