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
    /// typeof(<see cref="SavingSystem"/>)
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(SavingSystem))]
    public class SavingWrapper : MonoBehaviour
    {
        private const string DefaultSaveFile = "save";

        [SerializeField] float fadeInTime = 0.75f;

        #region Component References

        #region Required

        SavingSystem m_savingSystem;

        #endregion

        #endregion

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            m_savingSystem = GetComponent<SavingSystem>();
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html"/>
        /// </summary>
        private IEnumerator Start()
        {
            Fader fader = FindObjectOfType<Fader>();
            bool hasFader = fader != null;

            if (hasFader) fader.FadeOutImmediate();

            yield return m_savingSystem.LoadLastScene(DefaultSaveFile);

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
            m_savingSystem.Load(DefaultSaveFile);
        }

        public void Save()
        {
            m_savingSystem.Save(DefaultSaveFile);
        }
    }
}