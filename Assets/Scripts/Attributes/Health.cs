// Health.cs
// 07-05-2022
// James LaFritz

using RPG.Core;
using RPG.Saving;
using Unity.Mathematics;
using UnityEngine;
using static RPG.Core.StringReferences;

namespace RPG.Attributes
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

        private ActionScheduler m_actionScheduler;

        #endregion

        #region Optional

        /// <value>Cache the <a href="https://docs.unity3d.com/ScriptReference/Animator.html">UnityEngine.Animator</a></value>
        private Animator m_animator;

        private bool m_hasAnimator;
        private static int _dieHash;

        #endregion

        #endregion

        #region Properties

        /// <value>Is the GameObject dead. (m_value == 0)</value>
        public bool IsDead { get; private set; }

        #endregion

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            value = max;

            m_actionScheduler = GetComponent<ActionScheduler>();

            m_animator = GetComponentInChildren<Animator>();
            m_hasAnimator = m_animator != null;

            if (m_hasAnimator)
            {
                _dieHash = Animator.StringToHash(deathTrigger);
            }
        }

        #endregion

        #region Implementation of ISaveable

        /// <inheritdoc />
        public object CaptureState()
        {
            return value;
        }

        /// <inheritdoc />
        public void RestoreState(object state, int version)
        {
            value = (float)state;
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

            //Debug.Log($"{name} takes {damage} damage. Health is now {m_value}");

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
            m_actionScheduler.CancelCurrentAction();
            if (m_hasAnimator)
            {
                m_animator.SetTrigger(_dieHash);
            }
        }

        #endregion
    }
}