// SavingWrapper.cs
// 07-13-2022
// James LaFritz

using System.Collections;
using RPGEngine.Saving;
using UnityEngine;

namespace RPGEngine.SceneManagement
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// Is used to interface with the saving system.
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// typeof(<see cref="SaveSystem"/>)
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(SaveSystem))]
    public class SavingWrapper : MonoBehaviour
    {
        private const string DefaultSaveFile = "save";

        [SerializeField] float fadeInTime = 0.75f;

        #region Component References

        #region Required

        private SaveSystem _savingSystem;

        #endregion

        #endregion

        #region Unity Messages

        /// <summary>s
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            _savingSystem = GetComponent<SaveSystem>();
            StartCoroutine(LoadLastScene());
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(LoadLastScene());
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            
            if (Input.GetKeyDown(KeyCode.Delete))
                Delete();
        }

        #endregion

        public void Load()
        {
            _savingSystem.Load(DefaultSaveFile);
        }

        public void Save()
        {
            _savingSystem.Save(DefaultSaveFile);
        }

        public void Delete()
        {
            _savingSystem.Delete(DefaultSaveFile);
        }

        private IEnumerator LoadLastScene()
        {
            yield return _savingSystem.LoadLastScene(DefaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            if (!fader) yield break;
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
        }
    }
}