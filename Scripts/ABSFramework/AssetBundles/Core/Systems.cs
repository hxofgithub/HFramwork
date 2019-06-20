
using System.Collections.Generic;

namespace ABSFramework.Systems
{
    public class SystemManager : IInitailizeSystem, IExecuteSystem, ICleanupSystem
    {
        
        public SystemManager AddSystem(ISystem system)
        {
            var initialize = system as IInitailizeSystem;
            if (initialize != null)
                _initializeSystems.Add(initialize);

            var execute = system as IExecuteSystem;
            if (execute != null)
                _executeSystems.Add(execute);

            var cleanup = system as ICleanupSystem;
            if (cleanup != null)
                _cleanupSystems.Add(cleanup);

            return this;
        }

        public void Cleanup()
        {
            for (int i = 0, max = _cleanupSystems.Count; i < max; i++)
            {
                _cleanupSystems[i].Cleanup();
            }
        }

        public void Execute()
        {
            for (int i = 0, max = _executeSystems.Count; i < max; i++)
            {
                _executeSystems[i].Execute();
            }
        }

        public void Initailize()
        {
            for (int i = 0, max = _initializeSystems.Count; i < max; i++)
            {
                _initializeSystems[i].Initailize();
            }
        }
        
        private readonly List<IInitailizeSystem> _initializeSystems = new List<IInitailizeSystem>();
        private readonly List<ICleanupSystem> _cleanupSystems = new List<ICleanupSystem>();
        private readonly List<IExecuteSystem> _executeSystems = new List<IExecuteSystem>();
    }
}
