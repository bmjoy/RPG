// IAction.cs
// 07-05-2022
// James LaFritz

namespace RPG.Core
{
    /// <summary>
    /// Interface for Actions that can be performed by Game Objects in the Game.
    /// The Game Object Requires an <see cref="ActionScheduler"/> to perform the Action.
    /// </summary>
    public interface IAction
    {
        #region Public Methods

        /// <summary>
        /// Cancel The Action
        /// </summary>
        void Cancel();

        #endregion
    }
}