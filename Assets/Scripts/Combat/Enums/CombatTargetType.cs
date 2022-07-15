// CombatTargetType.cs
// 07-06-2022
// James LaFritz

namespace RPGEngine.Combat
{
    /// <summary>
    /// The Available Target Types
    /// </summary>
    public enum CombatTargetType
    {
        /// <summary>
        /// This <see cref="CombatTarget"/> is a player.
        /// </summary>
        Player,

        /// <summary>
        /// This <see cref="CombatTarget"/> is an enemy.
        /// </summary>
        Enemy
    }
}