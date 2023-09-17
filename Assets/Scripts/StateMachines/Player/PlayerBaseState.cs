public class PlayerBaseState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;

    public PlayerBaseState(Player player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void AnimationTriggerEvent(Player.AnimationTriggerType triggerType) { }

}
