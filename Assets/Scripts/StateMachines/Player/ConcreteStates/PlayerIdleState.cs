using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void AnimationTriggerEvent(Player.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        if (Mathf.Abs(player.WalkInput) > 0.5)
        {
            stateMachine.ChangeStage(player.WalkState);
        }
        else if (Mathf.Abs(player.WalkInput) > 0.5)
        {
            stateMachine.ChangeStage(player.WalkState);
        }
        if (player.IsJumpPressed) 
        {
            stateMachine.ChangeStage(player.JumpState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
