using Newtonsoft.Json.Linq;
using RPGEngine.Saving;
using UnityEngine;

namespace RPGEngine.Attributes
{
    public class Experience : MonoBehaviour, ISavable
    {
        [SerializeField] private float value;

        public void GainExperience(float amount) => value += amount;

        #region Implementation of IJsonSavable

        /// <inheritdoc />
        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(value);
        }

        /// <inheritdoc />
        public void RestoreFromJToken(JToken state, int version)
        {
            if (state == null) return;
            value = state.ToObject<float>();
        }

        #endregion
    }
}