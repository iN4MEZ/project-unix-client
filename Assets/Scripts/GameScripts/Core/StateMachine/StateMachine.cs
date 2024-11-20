namespace NMX
{
    public abstract class StateMachine
    {

        public IState currentState { get; private set; }

        public void ChangeState(IState state)
        {
            currentState?.Exit();

            currentState = state;

            currentState.Enter();
        }

        public void Enter()
        {
            currentState?.Enter();
        }

        public void Exit()
        {
            currentState?.Exit();
        }

        public void HandleInput()
        {
            currentState?.HandleInput();
        }

        public void OnAnimationEnterEvent()
        {
            currentState?.OnAnimationEnterEvent();
        }

        public void OnAnimationExitEvent()
        {
           currentState?.OnAnimationExitEvent();
        }

        public void OnAnimationTransitionEvent()
        {
            currentState?.OnAnimationTransitionEvent();
        }

        public void PhysicsUpdate()
        {
            currentState?.PhysicsUpdate();
        }

        public void Update()
        {
            currentState?.Update();
        }

        public void LateUpdate()
        {
            currentState?.LateUpdate();
        }
    }
}
