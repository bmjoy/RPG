// Fighter.cs
// 07-03-2022
// James LaFritz

using UnityEngine;

namespace RPG.Combat
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// A Fighter in the game.
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    public class Fighter : MonoBehaviour
    {
        /// <summary>
        /// Attack a target.
        /// </summary>
        /// <param name="target">The <see cref="CombatTarget"/> to attack.</param>
        public void Attack(CombatTarget target)
        {
            Debug.Log(name + " attacks " + target.name);
        }
    }
}