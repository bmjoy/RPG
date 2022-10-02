using System;
using UnityEngine;

namespace RPGEngine.Core
{
    public abstract class GameEvent<T> : ScriptableObject
    {
        private event Action<T> Event;

        public void RegisterListener(Action<T> action)
        {
            Event += action;
        }

        public void UnregisterListener(Action<T> action)
        {
            Event -= action;
        }
        
        public void Invoke(T arg1)
        {
            Event?.Invoke(arg1);
        }
    }
    
    public abstract class GameEvent<T1, T2> : ScriptableObject
    {
        private event Action<T1, T2> Event;

        public void RegisterListener(Action<T1, T2> action)
        {
            Event += action;
        }

        public void UnregisterListener(Action<T1, T2> action)
        {
            Event -= action;
        }
        
        public void Invoke(T1 arg1, T2 arg2)
        {
            Event?.Invoke(arg1, arg2);
        }
    }
}