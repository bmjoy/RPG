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
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html"/>
        /// </summary>
        private IEnumerator Start()
        {
            Fader fader = FindObjectOfType<Fader>();
            bool hasFader = fader != null;

            if (hasFader) fader.FadeOutImmediate();

            yield return _savingSystem.LoadLastScene(DefaultSaveFile);

            if (hasFader) yield return fader.FadeIn(fadeInTime);
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
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
    }
}