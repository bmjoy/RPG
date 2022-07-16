// CinematicTrigger.cs
// 07-07-2022
// James LaFritz

using Newtonsoft.Json.Linq;
using RPGEngine.Core;
using RPGEngine.Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPGEngine.Cinematics
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// triggers a <a href="https://docs.unity3d.com/ScriptReference/Playables.PlayableDirector.html">UnityEngine.Playables.PlayableDirector</a>
    /// to play a cut scene.
    /// <p>
    /// Implements
    /// <see cref="IJsonSavable"/>
    /// </p>
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// typeof(<a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/Playables.PlayableDirector.html">UnityEngine.Playables.PlayableDirector</a>)
    /// , typeof(<see cref="JsonSavableEntity"/>)
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(PlayableDirector), typeof(JsonSavableEntity))]
    public class CinematicTrigger : MonoBehaviour, IJsonSavable
    {
        #region Component References

        #region Required

        /// <value>Cache the <a href="https://docs.unity3d.com/ScriptReference/Playables.PlayableDirector.html">UnityEngine.Playables.PlayableDirector</a></value>
        private PlayableDirector m_director;

        #endregion

        #endregion

        [ReadOnly, SerializeField] private bool triggered;

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            m_director = GetComponent<PlayableDirector>();
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerEnter.html"/>
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (triggered || !other.CompareTag("Player")) return;
            triggered = true;
            m_director.Play();
        }

        #endregion

        #region Implementation of ISaveable

        /// <inheritdoc />
        public object CaptureState()
        {
            return triggered;
        }

        /// <inheritdoc />
        public void RestoreState(object state, int version)
        {
            triggered = (bool)state;
            if (!triggered) return;
            m_director.Play();
            m_director.time = m_director.duration;
        }

        #endregion

        #region Implementation of IJsonSavable

        /// <inheritdoc />
        public JToken CaptureAsJToken()
        {
            //return triggered;
            return null;
        }

        /// <inheritdoc />
        public void RestoreFromJToken(JToken state, int version)
        {
            // triggered = (bool)state;
            // if (!triggered) return;
            // m_director.Play();
            // m_director.time = m_director.duration;
        }

        #endregion
    }
}