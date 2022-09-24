using RPGEngine.Core;
using UnityEngine;

namespace RPGEngine.UI.Stats
{
    public abstract class StatDisplay : MonoBehaviour
    {
        [SerializeField] protected GameObject target;
        [SerializeField] private GameObjectFloatGameEvent statChangedEvent;

        public GameObject Target { set => target = value; } 

        protected virtual void OnEnable()
        {
            if (statChangedEvent) statChangedEvent.RegisterListener(OnStatValueChanged);
            //Debug.LogWarning($"{name}: On Enable");
        }

        protected virtual  void OnDisable()
        {
            if (statChangedEvent) statChangedEvent.UnregisterListener(OnStatValueChanged);
        }

        protected abstract void OnStatValueChanged(GameObject sender, float value);
    }
}