// RPGController.cs
// 07-06-2022
// James LaFritz

using RPGEngine.Attributes;
using RPGEngine.Combat;
using RPGEngine.Movement;
using UnityEngine;

namespace RPGEngine.Control
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that is the Base for all Controllers in Game.
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// typeof(<see cref="Mover"/>)
    /// , typeof(<see cref="Health"/>)
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(Mover), typeof(Health))]
    public abstract class RPGController : MonoBehaviour
    {
        #region Inspector Fields

        [SerializeField] protected CombatTargetType combatTargetType = CombatTargetType.Player;

        #endregion

        #region Component References

        #region Required

        /// <value>Cache the <see cref="Mover"/></value>
        protected Mover mover;

        /// <value>Cache the <see cref="Health"/></value>
        protected Health health;

        #endregion

        #region Optional

        /// <value>Cache the <see cref="Fighter"/></value>
        protected Fighter fighter;

        protected bool hasFighter;

        #endregion

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
            health = GetComponent<Health>();
        }

        #endregion
    }
}