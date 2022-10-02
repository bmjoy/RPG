// Fader.cs
// 07-12-2022
// James LaFritz

using System.Collections;
using UnityEngine;

namespace RPGEngine.SceneManagement
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
        private CanvasGroup _canvasGroup;
        private Coroutine _currentFade;

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        #endregion

        public void FadeOutImmediate()
        {
            _canvasGroup.alpha = 1;
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/Coroutine.html"/>
        /// </summary>
        public IEnumerator FadeOut(float time)
        {
            return Fade(1, time);
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/Coroutine.html"/>
        /// </summary>
        public IEnumerator FadeIn(float time)
        {
            return Fade(0, time);
        }

        public IEnumerator Fade(float target, float time)
        {
            if (_currentFade != null) StopCoroutine(_currentFade);
            _currentFade = StartCoroutine(FadeRoutine(target, time));
            yield return _currentFade;
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(_canvasGroup.alpha, target))
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }

            _canvasGroup.alpha = target;
        }
    }
}