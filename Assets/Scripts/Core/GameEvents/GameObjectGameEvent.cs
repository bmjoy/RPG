using System;
using UnityEngine;

namespace RPGEngine.Core
{
    [CreateAssetMenu(fileName = "GameObjectGameEvent", menuName = "RPG Project/Game Events/Game Object Game Event")]
    public class GameObjectGameEvent : ScriptableObject
    {
        private event Action<GameObject> Event;

        public void RegisterListener(Action<GameObject> action)
        {
            Event += action;
        }

        public void UnregisterListener(Action<GameObject> action)
        {
            Event -= action;
        }
        
        public void Invoke(GameObject gameObject)
        {
            Event?.Invoke(gameObject);
        }
    }
}