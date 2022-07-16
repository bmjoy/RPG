using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static RPGEngine.Saving.VersionControl;

namespace RPGEngine.Saving
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// provides the interface to the Json saving system.
    /// <p>
    /// Uses <a hfre="https://www.newtonsoft.com/json/help/html/Introduction.htm">Json.NET</a>
    /// to provide methods to save and restore all <see cref="JsonSavableEntity"/> in a scene.
    /// </p>
    /// <p>This component should be created once and shared between all subsequent scenes.</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    public class JsonSaveSystem : MonoBehaviour
    {
        private const string Extension = ".json";
        private const string SaveFolder = "Saves";

        public static string BasePath => Path.Combine(Application.persistentDataPath!, SaveFolder);

        #region Inspector Fields

        [Header("Config Data")]
        [Tooltip("Toggle this to true if you want the saving system to save state for inactive objects.")]
        [SerializeField]
        private bool includeInactive;

        #endregion

        #region Public Methods

        /// <summary>
        /// Will load the last scene that was saved and restore the state. This must be run as a coroutine.
        /// Loads the Scene in the background as the current Scene runs.
        /// </summary>
        /// <param name="saveFile">The save file name to consult for loading.</param>
        /// <returns>Waits for the Scene Manager to fully load the new scene.</returns>
        public IEnumerator LoadLastScene(string saveFile)
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            IDictionary<string, JToken> state = LoadFile(saveFile);
            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                buildIndex = (int)state["lastSceneBuildIndex"];
            }

            if (buildIndex == SceneManager.GetActiveScene().buildIndex)
            {
                RestoreState(state);
                yield break;
            }

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            RestoreState(state);
        }

        /// <summary>
        /// Save the current scene to the provided save file.
        /// </summary>
        /// <param name="saveFile">The name of the file to save to.</param>
        public void Save(string saveFile)
        {
            IDictionary<string, JToken> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }

        /// <summary>
        /// Load the scene from the provided save file.
        /// </summary>
        /// <param name="saveFile">The name of the save file to Load from.</param>
        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        /// <summary>
        /// Delete the state in the given save file.
        /// </summary>
        /// <param name="saveFile">The name of the save file to Delete</param>
        public void Delete(string saveFile)
        {
            string path = GetPath(saveFile);
            Debug.Log($"Deleting {path}");
            File.Delete(path!);
        }

        public IEnumerable<string> ListSaves()
        {
            return Directory.EnumerateFiles(BasePath!, "*" + Extension)
                .Select(Path.GetFileNameWithoutExtension);
        }

        #endregion

        #region Private Methods

        private IDictionary<string, JToken> LoadFile(string saveFile)
        {
            string path = GetPath(saveFile);
            if (!File.Exists(path)) return new JObject().ToObject<IDictionary<string, JToken>>();

            using StreamReader textReader = File.OpenText(path!);
            using JsonTextReader reader = new JsonTextReader(textReader);
            reader.FloatParseHandling = FloatParseHandling.Double;

            JObject stateObject = JObject.Load(reader);
            IDictionary<string, JToken> state = stateObject.ToObject<JObject>();

            int currentFileVersion = 0;
            if (state.ContainsKey("CurrentFileVersion"))
            {
                currentFileVersion = (int)state["CurrentFileVersion"];
            }

            if (currentFileVersion >= VersionControl.currentFileVersion || currentFileVersion >= minFileVersion)
                return state;

            Debug.LogWarning("Save file is from an older version of the game and is not supported. " +
                             $"Expected version: {VersionControl.currentFileVersion}, " +
                             $"Minimum Expected version: {minFileVersion}, " +
                             $"Current version: {currentFileVersion}");
            return new JObject().ToObject<IDictionary<string, JToken>>();
        }

        private void SaveFile(string saveFile, IDictionary<string, JToken> state)
        {
            string path = GetPath(saveFile);
            Debug.Log($"Saving to {path}");
            using StreamWriter textWriter = File.CreateText(path!);
            using JsonTextWriter writer = new JsonTextWriter(textWriter);
            writer.Formatting = Formatting.Indented;
            JObject.FromObject(state!).WriteTo(writer);
        }

        private void CaptureState(IDictionary<string, JToken> state)
        {
            state["CurrentFileVersion"] = currentFileVersion;
            foreach (JsonSavableEntity entity in FindObjectsOfType<JsonSavableEntity>(includeInactive))
            {
                state[entity.GetUniqueIdentifier()!] = entity.Capture();
            }

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreState(IDictionary<string, JToken> state)
        {
            int currentFileVersion = 0;
            if (state.ContainsKey("CurrentFileVersion"))
            {
                currentFileVersion = (int)state["CurrentFileVersion"];
            }

            foreach (JsonSavableEntity entity in FindObjectsOfType<JsonSavableEntity>(includeInactive))
            {
                string id = entity.GetUniqueIdentifier();
                if (!string.IsNullOrWhiteSpace(id) && state.ContainsKey(id))
                {
                    entity.Restore(state[id], currentFileVersion);
                }
            }
        }

        private string GetPath(string saveFile)
        {
            if (!Directory.Exists(BasePath)) Directory.CreateDirectory(BasePath!);

            return Path.Combine(BasePath, saveFile + Extension);
        }

        #endregion
    }
}
