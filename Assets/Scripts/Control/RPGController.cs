// RPGController.cs
// 07-06-2022
// James LaFritz

using RPG.Combat;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that is the Base for all Controllers in Game.
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// typeof(<see cref="Mover"/>)
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(Mover))]
    public abstract class RPGController : MonoBehaviour
    {
        #region Inspector Fields

        [SerializeField] protected float weaponRange = 2f;
        [SerializeField] protected float timeBetweenAttacks = 1f;
        [SerializeField] protected float weapondamage = 10f;

        [SerializeField] protected CombatTargetType combatTargetType = CombatTargetType.Player;

        #endregion

        #region Component References

        #region Required

        /// <value>Cache the <see cref="Mover"/></value>
        protected Mover mover;

        #endregion

        #region Optional

        /// <value>Cache the <see cref="Fighter"/></value>
        protected Fighter fighter;

        protected bool hasFighter;

        #endregion

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Are we moving
        /// </summary>
        /// <returns>True if we are moving, False if not.</returns>
        protected abstract bool IsMoving();

        /// <summary>
        /// Is the Controller Interacting with Combat.
        /// </summary>
        /// <returns>True if it is else return false.</returns>
        protected abstract bool IsInCombat();

        #endregion

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        protected virtual void Awake()
        {
            fighter = GetComponent<Fighter>();
            hasFighter = fighter != null;
            mover = GetComponent<Mover>();
            // ReSharper disable Unity.InefficientPropertyAccess
            if (mover != null) return;
            Debug.LogError($"{gameObject.name} requires a(n) {nameof(mover)} in order to work", gameObject);
            enabled = false;
            // ReSharper restore Unity.InefficientPropertyAccess
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        protected virtual void Update()
        {
            if (InCombat()) return;
            if (IsMoving()) return;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Are we in combat.
        /// </summary>
        /// <returns>if there is not a fighter component returns false, else returns IsInCombat()</returns>
        private bool InCombat()
        {
            if (!hasFighter) return false;

            return IsInCombat();
        }

        #endregion
    }
}