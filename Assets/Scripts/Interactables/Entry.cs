using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entry : Interactable
{
    [SerializeField] Transform destTransform;

    public override void HandleInteract()
    {
        if (!ControllingManager.Instance.IsControllingCat) return;

        ControllingManager.Instance.CatController.transform.position = destTransform.position;
    }
}
