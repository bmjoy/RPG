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

        #endregion
    }
}