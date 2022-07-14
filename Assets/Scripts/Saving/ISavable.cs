namespace RPG.Saving
{
    /// <summary>
    /// An Interface to Implement in any component that has state to save/restore.
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    public interface ISavable
    {
        /// <summary>
        /// Called when saving to capture the state of the component.
        /// </summary>
        /// <returns>The `System.Serializable`
        /// <a href="https://docs.microsoft.com/en-us/dotnet/api/System.Object?view=net-6.0">Object</a>
        /// that represents the state of the component
        /// </returns>
        object CaptureState();

        /// <summary>
        /// Called when restoring the state of a scene.
        /// </summary>
        /// <param name="state">The `System.Serializable`
        /// <a href="https://docs.microsoft.com/en-us/dotnet/api/System.Object?view=net-6.0">Object</a>
        /// that that was returned byCaptureState when saving.
        /// </param>
        /// <param name="version">The version number of the saved data.</param>
        void RestoreState(object state, int version);
    }
}