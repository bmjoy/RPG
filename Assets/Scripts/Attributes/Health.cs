// Health.cs
// 07-05-2022
// James LaFritz

using Newtonsoft.Json.Linq;
using RPGEngine.Core;
using RPGEngine.Saving;
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
    [RequireComponent(typeof(ActionScheduler))]
    public class Health : MonoBehaviour, ISavable
    {
        #region Inspector Fields

        /// <value>The maximum health of the object.</value>
        [SerializeField] float max = 100f;

        #endregion

        #region Private Fields

        [ReadOnly, SerializeField] private float value;

        #endregion

        #region Component References

        #region Required

        private ActionScheduler _actionScheduler;

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
            value = max;

            _actionScheduler = GetComponent<ActionScheduler>();

            _animator = GetComponentInChildren<Animator>();
            _hasAnimator = _animator != null;

            if (_hasAnimator)
            {
                _dieHash = Animator.StringToHash(DeathTrigger);
            }
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
            if (value <= 0)
            {
                value = 0;
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
        /// <param name="damage">The amount to reduce the health by.</param>
        public void TakeDamage(float damage)
        {
            if (IsDead) return;

            value = math.min(math.max(value - damage, 0), max);

            Debug.Log($"<color=blue>{name}:</color> " +
                      $"<color=darkblue>takes <color=red>{damage}</color> damage.</color> " +
                      $"<color=teal>Health is now <color=#38761d>{value}</color> / <color=#274e13>{max}</color></color>");

            if (value == 0)
            {
                Die();
            }
        }

        #endregion

        #region Private Methods

        private void Die()
        {
            if (IsDead) return;
            IsDead = true;
            _actionScheduler.CancelCurrentAction();
            if (_hasAnimator)
            {
                _animator.SetTrigger(_dieHash);
            }
        }

        #endregion
    }
}