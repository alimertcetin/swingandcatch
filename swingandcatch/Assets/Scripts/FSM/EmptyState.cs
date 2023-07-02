namespace TheGame.FSM
{
    public sealed class EmptyState : State
    {
        public EmptyState(StateMachine stateMachine) : base(stateMachine)
        {
            EnterState(null);
        }
    }
}