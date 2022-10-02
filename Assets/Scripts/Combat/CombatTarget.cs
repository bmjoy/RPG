// CombatTarget.cs
// 07-03-2022
// James LaFritz

using RPGEngine.Core;
using UnityEngine;

namespace RPGEngine.Combat
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// a Player can target./// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        #region Inspector Fields

        /// <value>The Type of <see cref="CombatTarget"/> this is.</value>
        [SerializeField] private CombatTargetType type = CombatTargetType.Enemy;

        #endregion

        #region Properties

        /// <value>The Type of <see cref="CombatTarget"/> this is.</value>
        public CombatTargetType Type => type;

        #endregion

        #region Implemtation IRaycastable

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(MonoBehaviour callingBehavior, RaycastHit hit)
        {
            Fighter fighter = callingBehavior.GetComponent<Fighter>();
            if (!fighter) return false;
            //Debug.Log($"{callingBehavior.name} has Fighter");
            if (!fighter.CanAttack(this)) return false;
            //Debug.Log($"{fighter.name} can Attack {target.name}");
            if (!Input.GetMouseButton(0)) return true;
            //Debug.Log($"{fighter.name} is Attacking {target.name}");
            fighter.Attack(this);
            return true;
        }

        #endregion
    }
}