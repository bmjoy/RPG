// Mover.cs
// 06-30-2022
// James LaFritz

using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using static RPG.Core.StringReferences;

namespace RPG.Movement
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// uses a <a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/AI.NavMeshAgent.html">UnityEngine.AI.NaveMeshAgent</a>
    /// to move a game object to a targets position.
    /// <p>
    /// Implements
    /// <see cref="IAction"/>
    /// </p>
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// typeof(<a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/AI.NavMeshAgent.html">UnityEngine.AI.NaveMeshAgent</a>)
    /// , typeof(<see cref="ActionScheduler"/>)
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(ActionScheduler))]
    public class Mover : MonoBehaviour, IAction
    {
        #region Component References

        #region Required

        /// <value>Cache the <see cref="ActionScheduler"/></value>
        private ActionScheduler m_actionScheduler;

        #endregion

        #region Optional

        /// <value>Cache the <a href="https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent.html">UnityEngine.AI.NaveMeshAgent</a></value>
        private NavMeshAgent m_navMeshAgent;

        private bool m_hasAgent;

        /// <value>Cache the <a href="https://docs.unity3d.com/ScriptReference/Animator.html">UnityEngine.Animator</a></value>
        private Animator m_animator;

        private bool m_hasAnimator;
        private static int _forwardSpeed;

        #endregion

        #endregion

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            m_animator = GetComponentInChildren<Animator>();
            m_hasAnimator = m_animator != null;

            if (m_hasAnimator)
            {
                _forwardSpeed = Animator.StringToHash(forwardSpeedFloat);
            }

            m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_hasAgent = m_navMeshAgent != null;

            m_actionScheduler = GetComponent<ActionScheduler>();

            if (m_hasAgent && m_actionScheduler != null) return;

            string errorObject = !m_hasAgent ? nameof(m_navMeshAgent) : nameof(m_actionScheduler);

            // ReSharper disable Unity.InefficientPropertyAccess
            Debug.LogError($"{gameObject.name} requires a(n) {errorObject} in order to work", gameObject);
            enabled = false;
            // ReSharper restore Unity.InefficientPropertyAccess
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            UpdateAnimator();
        }

        #endregion

        #region Implementation of IAction

        /// <inheritdoc />
        public void Cancel()
        {
            if (m_hasAgent)
            {
                m_navMeshAgent.isStopped = true;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Cancel any previous Actions and Move to the destination.
        /// </summary>
        /// <param name="destination"><a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/Vector3.html">UnityEngine.Vector3</a> To move To</param>
        public void StartMoveAction(Vector3 destination)
        {
            m_actionScheduler.StartAction(this);
            MoveTo(destination);
        }

        /// <summary>
        /// Move to the destination.
        /// </summary>
        /// <param name="destination"><a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/Vector3.html">UnityEngine.Vector3</a> To move To</param>
        public void MoveTo(Vector3 destination)
        {
            if (m_hasAgent)
            {
                MoveNavMeshAgentTo(destination);
            }
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
            if (!m_hasAgent) return;
            m_navMeshAgent!.destination = destination;
            m_navMeshAgent!.isStopped = false;
        }

        /// <summary>
        /// If there is an <a href="https://docs.unity3d.com/ScriptReference/Animator.html">UnityEngine.Animator</a>
        /// then set the "ForwardSpeed" of the Animator to the local velocity on the z access of the navmesh agent.
        /// </summary>
        private void UpdateAnimator()
        {
            if (!m_hasAnimator) return;

            Vector3 velocity = m_navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;

            m_animator.SetFloat(_forwardSpeed, speed);
        }

        #endregion
    }
}