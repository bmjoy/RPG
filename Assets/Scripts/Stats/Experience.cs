using System;
using Newtonsoft.Json.Linq;
using RPGEngine.Saving;
using UnityEngine;

namespace RPGEngine.Stats
{
    [RequireComponent(typeof(BaseStats))]
    public class Experience : MonoBehaviour, ISavable
    {
        #region Inspector Fields

        [SerializeField] private float value;

        #endregion

        #region Private Fields

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

        public float ExperienceToLevel => _baseStats.GetStatValue(Stat.ExperienceToLevel);

        #endregion

        #region Events

       public event Action OnExperienceGained;

        #endregion

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            _baseStats = GetComponent<BaseStats>();
        }

        private void Start()
        {
            GainExperience(0);
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

        public void GainExperience(float amount)
        {
            value += amount;
            OnExperienceGained?.Invoke();
        }

        public float GetPercentage()
        {
            return value / ExperienceToLevel;
        }

        #endregion
    }
}