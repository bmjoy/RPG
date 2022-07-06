// Health.cs
// 07-05-2022
// James LaFritz

using Unity.Mathematics;
using UnityEngine;
using static RPG.Core.StringReferences;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour
    {
        #region Inspector Fields

        /// <value>The maximum health of the object.</value>
        [SerializeField] float max = 100f;

        #endregion

        #region Private Fields

        private float m_value;

        #endregion

        #region Component References

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
            m_value = max;

            m_animator = GetComponentInChildren<Animator>();
            m_hasAnimator = m_animator != null;

            if (m_hasAnimator)
            {
                _dieHash = Animator.StringToHash(deathTrigger);
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

            m_value = math.min(math.max(m_value - damage, 0), max);

            //Debug.Log($"{name} takes {damage} damage. Health is now {m_value}");

            if (m_value == 0)
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
            if (m_hasAnimator)
            {
                m_animator.SetTrigger(_dieHash);
            }
        }

        #endregion
    }
}