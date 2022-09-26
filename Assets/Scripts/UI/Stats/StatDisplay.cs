using System;
using RPGEngine.Core;
using UnityEngine;

namespace RPGEngine.UI.Stats
{
    public abstract class StatDisplay : MonoBehaviour
    {
        [SerializeField] protected GameObject target;
        [SerializeField] private GameObjectFloatGameEvent statChangedEvent;

        public GameObject Target { set => target = value; }

        protected virtual void Awake()
        {
            if (statChangedEvent) statChangedEvent.RegisterListener(OnStatValueChanged);
        }

        protected virtual void OnDestroy()
        {
            if (statChangedEvent) statChangedEvent.UnregisterListener(OnStatValueChanged);
        }

        protected abstract void OnStatValueChanged(GameObject sender, float value);
    }
}