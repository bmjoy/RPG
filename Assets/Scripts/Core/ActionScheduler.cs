// ActionScheduler.cs
// 07-05-2022
// James LaFritz

using UnityEngine;

namespace RPG.Core
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// Stops and Starts the <see cref="IAction"/> that on the GameObject it is attached to.
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    public class ActionScheduler : MonoBehaviour
    {
        private IAction m_currentAction;

        #region Public Methods

        /// <summary>
        /// Starts an Action. Will cancel any other Actions that are currently running.
        /// </summary>
        /// <param name="action">The Action To Start.</param>
        public void StartAction(IAction action)
        {
            CancelCurrentAction(action);
            m_currentAction = action;
        }

        #endregion

        #region Private Methods

        private void CancelCurrentAction(IAction action)
        {
            if (m_currentAction == null || m_currentAction == action) return;
            m_currentAction.Cancel();
        }

        #endregion
    }
}