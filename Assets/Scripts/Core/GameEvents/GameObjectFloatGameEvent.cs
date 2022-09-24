using System;
using UnityEngine;

namespace RPGEngine.Core
{
    [CreateAssetMenu(fileName = "GameObjectFloatGameEvent", menuName = "RPG Project/Game Events/GameObject Float GameEvent")]
    public class GameObjectFloatGameEvent : ScriptableObject
    {
        private event Action<GameObject, float> Event;

        public void RegisterListener(Action<GameObject, float> action)
        {
            Event += action;
        }

        public void UnregisterListener(Action<GameObject, float> action)
        {
            Event -= action;
        }
        
        public void Invoke(GameObject sender, float amount)
        {
            Event?.Invoke(sender, amount);
        }
    }
}