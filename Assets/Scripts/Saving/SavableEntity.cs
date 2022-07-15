using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RPG.Saving
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a>
    /// for all game objects that need to save data.
    /// To be placed on any GameObject that has <see cref="ISavable"/> components that
    /// require saving.
    ///
    /// This class gives the GameObject a unique ID in the scene file. The ID is
    /// used for saving and restoring the state related to this GameObject. This
    /// ID can be manually override to link GameObjects between scenes (such as
    /// recurring characters, the player or a score board). Take care not to set
    /// this in a prefab unless you want to link all instances between scenes.
    /// <a href="https://docs.unity3d.com/ScriptReference/ExecuteAlways.html">UnityEngine.ExecuteAlways</a>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [ExecuteAlways]
    public class SavableEntity : MonoBehaviour
    {
        #region Inspector Fields

        [Header("Config Data")]
        [Tooltip("The unique ID is automatically generated in a scene file if " +
                 "left empty. Do not set in a prefab unless you want all instances to " +
                 "be linked.")]
        [SerializeField]
        private string uniqueIdentifier = "";

        #endregion

        /// <summary>
        /// <para>Cached State.</para>
        /// </summary>
        private static readonly Dictionary<string, SavableEntity> GlobalLookup = new Dictionary<string, SavableEntity>();

        #region Public Methods

        /// <summary>
        /// Get the Unique Identifier for this object.
        /// </summary>
        /// <returns>Unique Identifier for this object.</returns>
        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        /// <summary>
        /// Will capture the state of all `<see cref="ISavable"/>s` on this component.
        /// </summary>
        /// <returns>System.Serializable` object that represents this state.</returns>
        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (ISavable savable in GetComponents<ISavable>())
            {
                state[savable.GetType().ToString()!] = savable.CaptureState();
            }

            return state;
        }

        /// <summary>
        /// Will restore the state that was captured by `CaptureState`.
        /// </summary>
        /// <param name="state">The same object that was returned by `CaptureState`.</param>
        /// <param name="currentFileVersion">The current version of the save file.</param>
        public void RestoreState(object state, int currentFileVersion)
        {
            foreach (ISavable savable in GetComponents<ISavable>())
            {
                Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
                string typeString = savable.GetType().ToString()!;
                if (stateDict.ContainsKey(typeString))
                {
                    savable.RestoreState(stateDict[typeString], currentFileVersion);
                }
            }
        }

        #endregion

        #region Unity Messages (Unity Editor Only)

        #if UNITY_EDITOR
        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrWhiteSpace(gameObject.scene.path)) return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");

            if (string.IsNullOrWhiteSpace(property.stringValue) ||
                !IsUnique(property.stringValue))
            {
                property.stringValue = $"{name}-{System.Guid.NewGuid()}";
                serializedObject.ApplyModifiedProperties();
            }

            GlobalLookup[property.stringValue] = this;
        }
        #endif

        #endregion

        #region Private Methods

        private bool IsUnique(string candidate)
        {
            if (string.IsNullOrWhiteSpace(candidate)) return false;
            if (!GlobalLookup.ContainsKey(candidate)) return true;

            if (GlobalLookup[candidate] == this) return true;

            if (GlobalLookup[candidate] == null)
            {
                GlobalLookup.Remove(candidate);
                return true;
            }

            if (GlobalLookup[candidate].GetUniqueIdentifier() == candidate) return false;
            GlobalLookup.Remove(candidate);
            return true;
        }

        #endregion
    }
}