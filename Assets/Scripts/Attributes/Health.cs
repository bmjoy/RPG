// Health.cs
// 07-05-2022
// James LaFritz

using System.Collections;
using Newtonsoft.Json.Linq;
using RPGEngine.Core;
using RPGEngine.Saving;
using RPGEngine.Stats;
using Unity.Mathematics;
using UnityEngine;
using static RPGEngine.Core.StringReferences;

namespace RPGEngine.Attributes
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// represents The Health Attribute.
    /// <p>
    /// Implements
    /// <see cref="ISavable"/>
    /// </p>
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// typeof(<see cref="ActionScheduler"/>)
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(ActionScheduler), typeof(BaseStats))]
    public class Health : MonoBehaviour, ISavable
    {
        #region Inspector Fields

        [SerializeField, Range(0, 1)] private float healthRegenOnLevelUpPercent = .75f;

        #region Events

        [SerializeField] private GameObjectFloatGameEvent receivedDamage;
        [SerializeField] private GameObjectFloatGameEvent characterDied;
        [SerializeField] private GameObjectFloatGameEvent onHealthChanged;
        [SerializeField] private GameObjectFloatGameEvent onHealthMaxChanged;
        [SerializeField] private GameObjectGameEvent onLevelUp;
        
        #endregion

        #endregion

        #region Private Fields

        private float _value = -1;
        private float _max;

        #endregion

        #region Component References

        #region Required

        private ActionScheduler _actionScheduler;
        private BaseStats _baseStats;

        #endregion

        #region Optional

        /// <value>Cache the <a href="https://docs.unity3d.com/ScriptReference/Animator.html">UnityEngine.Animator</a></value>
        private Animator _animator;

        private bool _hasAnimator;
        private static int _dieHash;

        #endregion

        #endregion

        #region Properties

        /// <value>Is the GameObject dead. (_value == 0)</value>
        public bool IsDead { get; private set; }

        #endregion

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            _actionScheduler = GetComponent<ActionScheduler>();

            _animator = GetComponentInChildren<Animator>();
            _hasAnimator = _animator != null;

            if (_hasAnimator)
            {
                _dieHash = Animator.StringToHash(DeathTrigger);
            }

            _baseStats = GetComponent<BaseStats>();
        }

        private IEnumerator Start()
        {
            yield return null;
            
            _max = _baseStats.GetStatValue(Stat.Health);
            
            if (_value < 0) _value = _max;
            
            OnHealthChange();
        }

        private void OnEnable()
        {
            if (onLevelUp) onLevelUp.RegisterListener(RegenerateHealth);
            if (receivedDamage) receivedDamage.RegisterListener(TakeDamage);
        }

        private void OnDisable()
        {
            if (onLevelUp) onLevelUp.UnregisterListener(RegenerateHealth);
            if (receivedDamage) receivedDamage.UnregisterListener(TakeDamage);
        }

        #endregion

        #region Implementation of IJsonSavable

        /// <inheritdoc />
        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(_value);
        }

        /// <inheritdoc />
        public void RestoreFromJToken(JToken state, int version)
        {
            if (state == null) return;
            _value = state.ToObject<float>();
            if (_value <= 0)
            {
                _value = 0;
                Die();
            }
            else
            {
                IsDead = false;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// If the Game Object is already Dead do nothing.
        /// Reduce the value of the heath by the amount of damage.
        /// If the health is less than 0, set the health to 0 and set the Die trigger if there is an animator.
        /// </summary>
        /// <param name="gameObjectToReceiveDamage">The Game Object that receives the damage.</param>
        /// <param name="damage">The amount to reduce the health by.</param>
        private void TakeDamage(GameObject gameObjectToReceiveDamage, float damage)
        {
            if (IsDead) return;
            
            if (gameObjectToReceiveDamage != gameObject) return;

            _value = math.min(math.max(_value - damage, 0), _max);

            Debug.Log($"<color=blue>{name}:</color> <color=darkblue>takes <color=red>{damage}</color> damage.</color> " +
                      $"<color=teal>Health is now <color=#38761d>{_value}</color> / <color=#274e13>{_max}</color></color>");

            if(onHealthChanged) onHealthChanged.Invoke(gameObject, _value);

            if (_value != 0) return;
            Die();
        }

        public void OnHealthChange()
        {
            if (onHealthMaxChanged) onHealthMaxChanged.Invoke(gameObject, _max);
            if(onHealthChanged) onHealthChanged.Invoke(gameObject, _value);
        }

        #endregion

        #region Private Methods

        private void Die()
        {
            if (IsDead) return;
            IsDead = true;
            _actionScheduler.CancelCurrentAction();
            if (_hasAnimator) _animator.SetTrigger(_dieHash);
            if (characterDied) characterDied.Invoke(gameObject, GetAwardExp());
        }

        private float GetAwardExp()
        {
            float expAmount = 0;
            BaseStats stats = GetComponent<BaseStats>();
            if (stats) expAmount = stats.GetStatValue(Stat.ExperienceReward);

            return expAmount;
        }

        private void RegenerateHealth(GameObject sender)
        {
            if (sender != gameObject) return;

            var currentPercentage = IsDead ? 0 : _value / _max;
            _max = _baseStats.GetStatValue(Stat.Health);
            _value = Mathf.Max(_max * currentPercentage, _max * healthRegenOnLevelUpPercent);
            OnHealthChange();
        }

        #endregion
    }
}