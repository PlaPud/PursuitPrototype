using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatAiming : AimingController
{
    void Start()
    {
        EnemyAreaController.OnCombatStart += EnableAiming;
        EnemyAreaController.OnCombatEnd += DisableAiming;
    }

    private new void Update()
    {
        base.Update();
    }

    private void EnableAiming() 
    {
        List<SpriteRenderer> _allSprite = arrow.GetComponentsInChildren<SpriteRenderer>().ToList();
        _allSprite.ForEach(sprite => { sprite.enabled = true; });  
    }

    private void DisableAiming() 
    {
        List<SpriteRenderer> _allSprite = arrow.GetComponentsInChildren<SpriteRenderer>().ToList();
        _allSprite.ForEach(sprite => { sprite.enabled = false; });
    }

    private void OnDestroy()
    {
        EnemyAreaController.OnCombatStart -= EnableAiming;
        EnemyAreaController.OnCombatEnd -= DisableAiming;
    }
}
