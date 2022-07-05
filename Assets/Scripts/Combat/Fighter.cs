// Fighter.cs
// 07-03-2022
// James LaFritz

using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// is a Fighter in the game.
    /// Once the target is set by the attack method it will move towards the target and attack it.
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent(</a>
    /// <see cref="Mover"/>
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    public class Fighter : MonoBehaviour
    {
        [SerializeField] float weaponRange = 2f;

        private Mover m_mover;

        private Transform m_target;

        public bool IsAttacking { get; private set; }

        #region Unity Methods

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            m_mover = GetComponent<Mover>();
            // ReSharper disable Unity.InefficientPropertyAccess
            if (m_mover != null) return;
            Debug.LogError($"{gameObject.name} requires a(n) {nameof(m_mover)} in order to work", gameObject);
            enabled = false;
            // ReSharper restore Unity.InefficientPropertyAccess
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            if (!IsAttacking) return;

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

        #region Public Methods

        /// <summary>
        /// Attack a target.
        /// </summary>
        /// <param name="target">The <see cref="CombatTarget"/> to attack.</param>
        public void Attack(CombatTarget target)
        {
            m_target = target.transform;
            IsAttacking = m_target != null;
        }

        public void Cancel()
        {
            IsAttacking = false;
            m_target = null;
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
            if (!IsAttacking) return;
            m_mover.StopMovement();
            Debug.Log($"{name}  attacks  {m_target.name}");
        }

        #endregion
    }
}