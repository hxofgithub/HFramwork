using System;
using System.Collections.Generic;

namespace HFramwork
{
    public class StateManager : ISingleton
    {
        #region singleton
        public void Init()
        {
            _stateDict = new Dictionary<string, BaseState>();
        }

        public void Dispose()
        {
            if (CurrentState != null)
            {
                CurrentState.ExitState();
                CurrentState = null;
            }
            _stateDict = null;
            Singleton<StateManager>.Dispose();
        }

        #endregion


        public BaseState CurrentState{ get; private set;}

        public void Update()
        {
            if (CurrentState != null)
                CurrentState.ExcuteState();
        }

        public void ChangeState(string stateName)
        {
            BaseState bs = GetState(stateName);
            if (bs != null)
            {
                if (CurrentState != null)
                    CurrentState.ExitState();

                bs.EnterState();

                CurrentState = bs;
            }
        }

        public BaseState GetState(string stateName)
        {
            var state = _stateDict.GetValue(stateName);
            if (state == null)
            {
                state = CreateNew(stateName);
                _stateDict[stateName] = state;
            }            
            return state;
        }

        private BaseState CreateNew(string stateName)
        {
            Type t = Type.GetType(stateName);
            if (t == null)
            {
                Log.Error("error state:{0}", stateName);
                return null;
            }

            var state = Activator.CreateInstance(t) as BaseState;
            if (state == null)
            {
                Log.Error("error state:{0}", stateName);
                return null;
            }

            return state;
        }

        private Dictionary<string,BaseState> _stateDict;

    }

}
