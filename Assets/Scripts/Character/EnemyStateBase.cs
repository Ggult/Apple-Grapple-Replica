namespace AppleGrapple
{
    public abstract class EnemyStateBase
    {
        protected readonly EnemyStateMachine StateMachine;

        protected EnemyStateBase(EnemyStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }

        public abstract void Enter();
        public abstract void Exit();
        public abstract void Update();
    }
}
