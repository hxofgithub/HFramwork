namespace HFramework
{
    public abstract class BaseState
    {
        public abstract void EnterState();
        public abstract void ExcuteState();
        public abstract void ExitState();
    }
}