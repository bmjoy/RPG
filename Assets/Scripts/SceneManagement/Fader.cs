// Fader.cs
// 07-12-2022
// James LaFritz

using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// Fades a Canvas Group In and Out.
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// typeof(<a href="https://docs.unity3d.com/ScriptReference/CanvasGroup.html">UnityEngine.CanvasGroup</a>)
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class Fader : MonoBehaviour
    {
        private CanvasGroup m_canvasGroup;

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
        }

        #endregion

        public void FadeOutImmediate()
        {
            m_canvasGroup.alpha = 1;
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/Coroutine.html"/>
        /// </summary>
        public IEnumerator FadeOut(float time)
        {
            while (m_canvasGroup.alpha < 1)
            {
                m_canvasGroup.alpha += Time.deltaTime / time;
                yield return null;
            }

            m_canvasGroup.alpha = 1f;
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/Coroutine.html"/>
        /// </summary>
        public IEnumerator FadeIn(float time)
        {
            while (m_canvasGroup.alpha > 0)
            {
                m_canvasGroup.alpha -= Time.deltaTime / time;
                yield return null;
            }

            m_canvasGroup.alpha = 0f;
        }
    }
}