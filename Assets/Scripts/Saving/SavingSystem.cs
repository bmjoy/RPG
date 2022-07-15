using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using static RPG.Saving.VersionControl;

namespace RPG.Saving
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// provides the interface to the saving system. It provides methods to save and restore a scene.
    ///
    /// This component should be created once and shared between all subsequent scenes.
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    public class SavingSystem : MonoBehaviour
    {
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
            Dictionary<string, object> state = LoadFile(saveFile);
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
            Dictionary<string, object> state = LoadFile(saveFile);
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

        #endregion

        #region Private Methods

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPath(saveFile);
            if (!File.Exists(path)) return new Dictionary<string, object>();

            BinaryFormatter formatter = new BinaryFormatter();
            using FileStream stream = File.Open(path!, FileMode.Open);

            Dictionary<string, object> state = (Dictionary<string, object>)formatter.Deserialize(stream);

            int currentFileVersion = 0;
            if (state.ContainsKey("CurrentFileVersion"))
            {
                currentFileVersion = (int)state["CurrentFileVersion"];
            }

            if (currentFileVersion >= VersionControl.currentFileVersion || currentFileVersion >= minFileVersion)
                return state;

            Debug.LogWarning($"Save file is from an older version of the game and is not supported. " +
                             $"Expected version: {VersionControl.currentFileVersion}, " +
                             $"Minimum Expected version: {minFileVersion}, " +
                             $"Current version: {currentFileVersion}");
            return new Dictionary<string, object>();
        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetPath(saveFile);
            Debug.Log($"Saving to {path}");
            using FileStream stream = File.Open(path!, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, state!);
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            state["CurrentFileVersion"] = currentFileVersion;
            foreach (SavableEntity entity in FindObjectsOfType<SavableEntity>(includeInactive))
            {
                state[entity.GetUniqueIdentifier()!] = entity.CaptureState();
            }

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            int currentFileVersion = 0;
            if (state.ContainsKey("CurrentFileVersion"))
            {
                currentFileVersion = (int)state["CurrentFileVersion"];
            }

            foreach (SavableEntity entity in FindObjectsOfType<SavableEntity>(includeInactive))
            {
                string id = entity.GetUniqueIdentifier();
                if (!string.IsNullOrWhiteSpace(id) && state.ContainsKey(id))
                {
                    entity.RestoreState(state[id], currentFileVersion);
                }
            }
        }

        private string GetPath(string saveFile)
        {
            string basePath = Path.Combine(Application.persistentDataPath!, "GameData");

            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

            return Path.Combine(basePath, saveFile + ".sav");
        }

        #endregion
    }
}