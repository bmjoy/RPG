using Newtonsoft.Json.Linq;
using RPGEngine.Saving;
using RPGEngine.Stats;
using UnityEngine;

namespace RPGEngine.Attributes
{
    [RequireComponent(typeof(BaseStats))]
    public class Experience : MonoBehaviour, ISavable
    {
        #region Inspector Fields

        [SerializeField] private float value;

        #endregion

        #region Component References

        #region Required

        private BaseStats _baseStats;

        #endregion

        #region Optional

        #endregion

        #endregion

        #region Properties

        public float Value => value;

        public float ExperienceToNextLevel { get; private set; } = 100;

        #endregion

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            _baseStats = GetComponent<BaseStats>();
        }

        #endregion

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

        #region Public Methods

        public void GainExperience(float amount) => value += amount;

        public float GetPercentage()
        {
            return value / ExperienceToNextLevel;
        }

        #endregion
    }
}