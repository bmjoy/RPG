// Mover.cs
// 06-30-2022
// James LaFritz

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPGEngine.Attributes;
using RPGEngine.Core;
using RPGEngine.Saving;
using UnityEngine;
using UnityEngine.AI;
using static RPGEngine.Core.StringReferences;

namespace RPGEngine.Movement
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// uses a <a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/AI.NavMeshAgent.html">UnityEngine.AI.NaveMeshAgent</a>
    /// to move a game object to a targets position.
    /// <p>
    /// Implements
    /// <see cref="IAction"/>
    /// <see cref="ISavable"/>
    /// </p>
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// typeof(<a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/AI.NavMeshAgent.html">UnityEngine.AI.NaveMeshAgent</a>)
    /// , typeof(<see cref="ActionScheduler"/>)
    /// , typeof(<see cref="Health"/>)
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(ActionScheduler), typeof(Health))]
    public class Mover : MonoBehaviour, IAction, ISavable
    {
        [SerializeField] private BoolGameEvent gamePausedEvent;
        private bool _gamePaused;
        
        #region Component References

        #region Required

        /// <value>Cache the <see cref="ActionScheduler"/></value>
        private ActionScheduler _actionScheduler;

        /// <value>Cache the <see cref="Health"/></value>
        private Health _health;

        #endregion

        #region Optional

        /// <value>Cache the <a href="https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent.html">UnityEngine.AI.NaveMeshAgent</a></value>
        private NavMeshAgent _navMeshAgent;

        private bool _hasAgent;

        /// <value>Cache the <a href="https://docs.unity3d.com/ScriptReference/Animator.html">UnityEngine.Animator</a></value>
        private Animator _animator;

        private bool _hasAnimator;
        private static int _forwardSpeed;

        #endregion

        #endregion

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _hasAnimator = _animator != null;

            if (_hasAnimator)
            {
                _forwardSpeed = Animator.StringToHash(ForwardSpeedFloat);
            }

            _navMeshAgent = GetComponent<NavMeshAgent>();
            _hasAgent = _navMeshAgent != null;

            _actionScheduler = GetComponent<ActionScheduler>();

            _health = GetComponent<Health>();
        }
        private void OnEnable()
        {
            if (gamePausedEvent) gamePausedEvent.RegisterListener(OnPauseGame);
        }

        private void OnDisable()
        {
            
            if (gamePausedEvent) gamePausedEvent.UnregisterListener(OnPauseGame);
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            if (_gamePaused) return;
            if (_navMeshAgent.isActiveAndEnabled && _health.IsDead)
            {
                _navMeshAgent.destination = transform.position;
                _navMeshAgent.ResetPath();
                _navMeshAgent.velocity = Vector3.zero;
                _navMeshAgent.isStopped = true;
                UpdateAnimator();
                _navMeshAgent.enabled = false;
            }

            if (_health.IsDead) return;

            UpdateAnimator();
        }

        #endregion

        #region Implementation of IAction

        /// <inheritdoc />
        public void Cancel()
        {
            if (_hasAgent)
            {
                _navMeshAgent.isStopped = true;
            }
        }

        #endregion

        #region Saving and Loading

        [System.Serializable]
        private struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        private struct MoverLoadData
        {
            public Vector3 Position;
            public Vector3 Rotation;
        }

        #region Implementation of IJsonSavable

        /// <inheritdoc />
        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            stateDict["Position"] = transform.position.ToToken();
            stateDict["Rotation"] = transform.eulerAngles.ToToken();
            return state;
        }

        /// <inheritdoc />
        public void RestoreFromJToken(JToken state, int version)
        {
            if (state == null || version < 4) return;
            if (_hasAgent) _navMeshAgent.enabled = false;

            MoverLoadData data = new();
            switch (version)
            {
                case 4:
                {
                    MoverSaveData sd = state.ToObject<MoverSaveData>();
                    data.Position = sd.position.ToVector();
                    data.Rotation = sd.rotation.ToVector();
                    break;
                }
                case > 4:
                    data = new MoverLoadData()
                    {
                        Position = state.ToObject<MoverLoadData>().Position,
                        Rotation = state.ToObject<MoverLoadData>().Rotation
                    };
                    break;
            }

            Transform transform1 = transform;
            transform1.position = data.Position;
            transform1.eulerAngles = data.Rotation;

            if (_hasAgent) _navMeshAgent.enabled = true;
        }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Cancel any previous Actions and Move to the destination.
        /// </summary>
        /// <param name="destination"><a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/Vector3.html">UnityEngine.Vector3</a> To move To</param>
        public void StartMoveAction(Vector3 destination)
        {
            _actionScheduler.StartAction(this);
            MoveTo(destination);
        }

        /// <summary>
        /// Move to the destination.
        /// </summary>
        /// <param name="destination"><a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/Vector3.html">UnityEngine.Vector3</a> To move To</param>
        public void MoveTo(Vector3 destination)
        {
            if (_hasAgent)
            {
                MoveNavMeshAgentTo(destination);
            }
        }

        public void SetMoveSpeed(float speed)
        {
            if (!_hasAgent) return;
            _navMeshAgent.speed = speed;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Tell the <a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/AI.NavMeshAgent.html">UnityEngine.AI.NaveMeshAgent</a>
        /// to move to the destination.
        /// </summary>
        /// <param name="destination"><a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/Vector3.html">UnityEngine.Vector3</a> To move To</param>
        private void MoveNavMeshAgentTo(Vector3 destination)
        {
            if (!_hasAgent) return;
            _navMeshAgent!.destination = destination;
            _navMeshAgent!.isStopped = false;
        }

        /// <summary>
        /// If there is an <a href="https://docs.unity3d.com/ScriptReference/Animator.html">UnityEngine.Animator</a>
        /// then set the "ForwardSpeed" of the Animator to the local velocity on the z access of the navmesh agent.
        /// </summary>
        private void UpdateAnimator()
        {
            if (!_hasAnimator) return;

            Vector3 velocity = _navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;

            _animator.SetFloat(_forwardSpeed, speed);
        }

        private void OnPauseGame(bool paused)
        {
            _gamePaused = paused;
        }

        #endregion
    }
}