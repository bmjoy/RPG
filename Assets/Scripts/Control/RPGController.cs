// RPGController.cs
// 07-06-2022
// James LaFritz

using RPGEngine.Attributes;
using RPGEngine.Combat;
using RPGEngine.Core;
using RPGEngine.Movement;
using UnityEngine;

namespace RPGEngine.Control
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that is the Base for all Controllers in Game.
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// typeof(<see cref="Movement.Mover"/>)
    /// , typeof(<see cref="Attributes.Health"/>)
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(Mover), typeof(Health))]
    public abstract class RPGController : MonoBehaviour
    {

        #region Inspector Fields

        [SerializeField] private BoolGameEvent gamePausedEvent;
        [SerializeField] protected CombatTargetType combatTargetType = CombatTargetType.Player;

        #endregion

        #region Component References

        #region Required

        /// <value>Cache the <see cref="Movement.Mover"/></value>
        protected Mover Mover;

        /// <value>Cache the <see cref="Attributes.Health"/></value>
        protected Health Health;

        #endregion

        #region Optional

        /// <value>Cache the <see cref="Combat.Fighter"/></value>
        protected Fighter Fighter;

        protected bool HasFighter;

        #endregion

        #endregion

        protected bool GamePaused = true;


        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        protected virtual void Awake()
        {
            Fighter = GetComponent<Fighter>();
            HasFighter = Fighter != null;
            Mover = GetComponent<Mover>();
            Health = GetComponent<Health>();
        }

        protected virtual void OnEnable()
        {
            if (gamePausedEvent) gamePausedEvent.RegisterListener(OnGamePaused);
        }

        protected virtual void OnDisable()
        {
            if (gamePausedEvent) gamePausedEvent.UnregisterListener(OnGamePaused);
        }

        #endregion

        private void OnGamePaused(bool paused)
        {
            GamePaused = paused;
        }
    }
}