// JsonSavableEntity.cs
// 07-15-2022
// James LaFritz

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace RPGEngine.Saving
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a>
    /// for all game objects that need to save data.
    /// To be placed on any GameObject that has <see cref="IJsonSavable"/> components that
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
    public class JsonSavableEntity : MonoBehaviour
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
        private static readonly Dictionary<string, JsonSavableEntity> GlobalLookup = new Dictionary<string,
            JsonSavableEntity>();

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
        /// Will capture the state of all `<see cref="IJsonSavable"/>s` on this component.
        /// </summary>
        /// <returns><a href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JToken.htm">JToken</a> object that represents the state.</returns>
        public JToken Capture()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            foreach (IJsonSavable jsonSavable in GetComponents<IJsonSavable>())
            {
                JToken token = jsonSavable.CaptureAsJToken();
                string component = jsonSavable.GetType().ToString();
                LogToken(component, token);
                stateDict[jsonSavable.GetType().ToString()!] = jsonSavable.CaptureAsJToken();
            }

            return state;
        }

        /// <summary>
        /// Will restore the state of all `<see cref="IJsonSavable"/>s` on this component that was captured by `CaptureState`.
        /// </summary>
        /// <param name="state"><a href="https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_Linq_JToken.htm">JToken</a> object that represents the state of the entity.</param>
        /// <param name="currentFileVersion">The current version of the save file.</param>
        public void Restore(JToken state, int currentFileVersion)
        {
            IDictionary<string, JToken> stateDict = state.ToObject<JObject>();
            foreach (IJsonSavable jsonSavable in GetComponents<IJsonSavable>())
            {
                string component = jsonSavable.GetType().ToString();
                if (!stateDict.ContainsKey(component!)) continue;
                JToken token = stateDict[component];
                LogToken(component, token);
                jsonSavable.RestoreFromJToken(token, currentFileVersion);
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

        private void LogToken(string component, JToken token)
        {
            Debug.Log(
                $"<color=blue>{name}:</color> <color=brown>Capture:</color> <color=darkblue>{component}</color> = <color=teal>{token}</color>");
        }


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