public class PlayerStateMachine
{
    public PlayerBaseState CurrentState { get; set; }

    public void Initialize(PlayerBaseState startingState) 
    {
        CurrentState = startingState;
        CurrentState.EnterState();
    }

    public void ChangeStage(PlayerBaseState newState) 
    {
        CurrentState.ExitState();
        CurrentState = newState;
        CurrentState.EnterState();
    }
}
