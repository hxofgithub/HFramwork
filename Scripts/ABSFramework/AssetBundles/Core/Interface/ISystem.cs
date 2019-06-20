
namespace ABSFramework.Systems
{
    public interface ISystem { }

    public interface IExecuteSystem : ISystem
    {
        void Execute();
    }

    public interface IInitailizeSystem : ISystem
    {
        void Initailize();
    }

    public interface ICleanupSystem : ISystem
    {
        void Cleanup();
    }

}

