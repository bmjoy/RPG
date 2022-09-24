using System.Collections;
using Newtonsoft.Json.Linq;
using RPGEngine.Core;
using RPGEngine.Saving;
using UnityEngine;

namespace RPGEngine.Stats
{
    [RequireComponent(typeof(BaseStats))]
    public class Experience : MonoBehaviour, ISavable
    {
        #region Inspector Fields

        #region Events

        [SerializeField] private GameObjectFloatGameEvent onExperiencedChanged;
        [SerializeField] private GameObjectFloatGameEvent characterDied;

        #endregion

        #endregion

        #region Private Fields

        #endregion

        #region Properties

        public float Value { get; private set; }

        #endregion

        #region Unity Messages

        private IEnumerator Start()
        {
            yield return null;
            if (onExperiencedChanged) onExperiencedChanged.Invoke(gameObject, Value);
        }

        private void OnEnable()
        {
            if (characterDied) characterDied.RegisterListener(GainExperience);
        }

        private void OnDisable()
        {
            if (characterDied) characterDied.UnregisterListener(GainExperience);
        }

        #endregion

        #region Implementation of IJsonSavable

        /// <inheritdoc />
        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(Value);
        }

        /// <inheritdoc />
        public void RestoreFromJToken(JToken state, int version)
        {
            if (state == null) return;
            Value = state.ToObject<float>();
        }

        #endregion

        #region Private Methods

        private void GainExperience(GameObject sender, float amount)
        {
            if (sender == gameObject) return;
            Value += amount;
            if (onExperiencedChanged) onExperiencedChanged.Invoke(gameObject, Value);
        }

        #endregion
    }
}