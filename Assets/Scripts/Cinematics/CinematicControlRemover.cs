// CinematicControlRemover.cs
// 07-07-2022
// James LaFritz

using RPGEngine.Control;
using RPGEngine.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPGEngine.Cinematics
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
        private PlayableDirector _director;

        #endregion

        #endregion

        private ActionScheduler _actionScheduler;
        private PlayerController _controller;
        private bool _hasPlayerController;

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            _director = GetComponent<PlayableDirector>();
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnEnable.html"/>
        /// </summary>
        private void OnEnable()
        {
            _director.played += DisableControl;
            _director.stopped += EnableControl;
            _director.paused += EnableControl;
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDisable.html"/>
        /// </summary>
        private void OnDisable()
        {
            _director.played -= DisableControl;
            _director.stopped -= EnableControl;
        }

        #endregion

        private void DisableControl(PlayableDirector director)
        {
            if (director != _director) return;
            Debug.Log($"{director.name} is playing.");
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) return;
            _actionScheduler = player.GetComponent<ActionScheduler>();
            if (_actionScheduler != null) _actionScheduler.CancelCurrentAction();
            _controller = player.GetComponent<PlayerController>();
            if (_controller != null) _controller.enabled = false;
        }

        private void EnableControl(PlayableDirector director)
        {
            if (director != _director) return;
            Debug.Log($"{director.name} is stopped.");
            if (_controller != null) _controller.enabled = true;
        }
    }
}