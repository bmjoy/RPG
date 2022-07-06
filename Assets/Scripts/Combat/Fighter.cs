// Fighter.cs
// 07-03-2022
// James LaFritz

using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using static RPG.Core.StringReferences;

namespace RPG.Combat
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// is a Fighter in the game.
    /// Once the target is set by the attack method it will move towards the target and attack it.
    /// <p>
    /// Implements
    /// <see cref="IAction"/>
    /// </p>
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// typeof(<see cref="Mover"/>)
    /// , typeof(<see cref="ActionScheduler"/>)
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(Mover), typeof(ActionScheduler))]
    public class Fighter : MonoBehaviour, IAction
    {
        #region Inspector Fields

        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weapondamage = 10f;

        #endregion

        #region Private Fields

        private Transform m_target;
        private bool m_hasTarget;

        private float m_timeSinceLastAttack;

        #endregion

        #region Component References

        #region Required

        /// <value>Cache the <see cref="Mover"/></value>
        private Mover m_mover;

        /// <value>Cache the <see cref="ActionScheduler"/></value>
        private ActionScheduler m_actionScheduler;

        #endregion

        #region Optional

        /// <value>Cache the <a href="https://docs.unity3d.com/ScriptReference/Animator.html">UnityEngine.Animator</a></value>
        private Animator m_animator;

        private bool m_hasAnimator;
        private static int _attackHash;

        #endregion

        #endregion

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            m_mover = GetComponent<Mover>();
            m_actionScheduler = GetComponent<ActionScheduler>();

            m_animator = GetComponentInChildren<Animator>();
            m_hasAnimator = m_animator != null;

            if (m_hasAnimator)
            {
                _attackHash = Animator.StringToHash(attackTrigger);
            }

            string errorObject = "";
            if (m_mover == null) errorObject = nameof(m_mover);
            if (m_actionScheduler == null) errorObject += nameof(m_actionScheduler);
            if (string.IsNullOrEmpty(errorObject)) return;

            // ReSharper disable Unity.InefficientPropertyAccess
            Debug.LogError($"{gameObject.name} requires a(n) {errorObject} in order to work", gameObject);
            enabled = false;
            // ReSharper restore Unity.InefficientPropertyAccess
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            m_timeSinceLastAttack += Time.deltaTime;

            if (!m_hasTarget) return;

            if (!GetIsInRange(m_target))
            {
                m_mover.MoveTo(m_target.position);
            }
            else
            {
                AttackBehavior();
            }
        }

        #endregion

        #region Implementation of IAction

        /// <inheritdoc />
        public void Cancel()
        {
            m_hasTarget = false;
            m_target = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attack a target.
        /// </summary>
        /// <param name="target">The <see cref="CombatTarget"/> to attack.</param>
        public void Attack(CombatTarget target)
        {
            m_actionScheduler.StartAction(this);
            m_target = target.transform;
            m_hasTarget = m_target != null;
        }

        #endregion

        #region Private Methods

        private bool GetIsInRange(Transform targetTransform)
        {
            return targetTransform != null &&
                   Vector3.Distance(transform.position, targetTransform.position) < weaponRange;
        }

        private void AttackBehavior()
        {
            if (!m_hasTarget) return;
            m_mover.Cancel();

            if (m_timeSinceLastAttack < timeBetweenAttacks) return;
            m_timeSinceLastAttack = 0;
            if (m_hasAnimator)
            {
                // This will trigger Hit() from the Animation Event.
                m_animator.SetTrigger(_attackHash);
            }
            else
            {
                Hit();
            }
        }

        /// <summary>
        /// Need for Animation Event.
        /// Will Damage the target in sync with the animation.
        /// </summary>
        private void Hit()
        {
            if (!m_hasTarget) return;

            Health targetHealth = m_target.GetComponent<Health>();
            if (targetHealth == null) return;
            targetHealth.TakeDamage(weapondamage);
        }

        #endregion
    }
}