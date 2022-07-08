// CinematicControlRemover.cs
// 07-07-2022
// James LaFritz

using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// Observes <a href="https://docs.unity3d.com/ScriptReference/Playables.PlayableDirector.html">UnityEngine.Playables.PlayableDirector</a>
    /// to remove control from other scripts when the cinematic is playing.
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// typeof(<a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/Playables.PlayableDirector.html">UnityEngine.Playables.PlayableDirector</a>)
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(PlayableDirector))]
    public class CinematicControlRemover : MonoBehaviour
    {
        #region Component References

        #region Required

        /// <value>Cache the <a href="https://docs.unity3d.com/ScriptReference/Playables.PlayableDirector.html">UnityEngine.Playables.PlayableDirector</a></value>
        private PlayableDirector m_director;

        #endregion

        #endregion

        private ActionScheduler m_actionScheduler;
        private PlayerController m_controller;
        private bool m_hasPlayerController;

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            m_director = GetComponent<PlayableDirector>();
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnEnable.html"/>
        /// </summary>
        private void OnEnable()
        {
            m_director.played += DisableControl;
            m_director.stopped += EnableControl;
            m_director.paused += EnableControl;
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDisable.html"/>
        /// </summary>
        private void OnDisable()
        {
            m_director.played -= DisableControl;
            m_director.stopped -= EnableControl;
        }

        #endregion

        private void DisableControl(PlayableDirector director)
        {
            if (director != m_director) return;
            Debug.Log($"{director.name} is playing.");
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) return;
            m_actionScheduler = player.GetComponent<ActionScheduler>();
            if (m_actionScheduler != null) m_actionScheduler.CancelCurrentAction();
            m_controller = player.GetComponent<PlayerController>();
            if (m_controller != null) m_controller.enabled = false;
        }

        private void EnableControl(PlayableDirector director)
        {
            if (director != m_director) return;
            Debug.Log($"{director.name} is stopped.");
            if (m_controller != null) m_controller.enabled = true;
        }
    }
}