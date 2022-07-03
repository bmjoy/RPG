// Mover.cs
// 06-30-2022
// James LaFritz

using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// uses a <a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/AI.NavMeshAgent.html">UnityEngine.AI.NaveMeshAgent</a>
    /// to move a game object to a targets position.
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent(</a>
    /// <a href="https://docs.unity3d.com/2021.3/Documentation/ScriptReference/AI.NavMeshAgent.html">typeof(UnityEngine.AI.NaveMeshAgent)</a>
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class Mover : MonoBehaviour
    {
        /// <value>The target to move to.</value>>
        [Tooltip("The Target to move too.")]
        [SerializeField]
        private Transform target;

        /// <value>Cache the <a href="https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent.html">UnityEngine.AI.NaveMeshAgent</a></value>
        private NavMeshAgent m_navMeshAgent;

        /// <value>Cache the <a href="https://docs.unity3d.com/ScriptReference/Animator.html">UnityEngine.Animator</a></value>
        private Animator m_animator;

        private bool m_hasAnimator;
        private static int _forwardSpeed;

        #region Unity Methods

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            m_animator = GetComponentInChildren<Animator>();
            m_hasAnimator = m_animator != null;

            if (m_hasAnimator)
            {
                _forwardSpeed = Animator.StringToHash("ForwardSpeed");
            }

            // Cache the NavMeshAgent component.
            m_navMeshAgent = GetComponent<NavMeshAgent>();

            // ReSharper disable Unity.InefficientPropertyAccess
            // If the GameObject has a NavMeshAgent component return.
            if (m_navMeshAgent != null) return;
            // Otherwise Log an error noting that the GameObject does not have a NavMeshAgent component.
            Debug.LogError($"{gameObject.name} requires a(n) {nameof(m_navMeshAgent)} in order to work", gameObject);
            // Disable the GameObject.
            enabled = false;
            // ReSharper restore Unity.InefficientPropertyAccess
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            // If the Left Mouse Button is pressed.
            if (Input.GetMouseButton(0))
            {
                MoveToCursor();
            }

            UpdateAnimator();
        }

        #endregion

        private void MoveToCursor()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hasHIt = Physics.Raycast(ray, out RaycastHit hit);
            if (hasHIt)
            {
                m_navMeshAgent!.destination = hit.point;
            }
        }

        private void UpdateAnimator()
        {
            if (!m_hasAnimator) return;

            Vector3 velocity = m_navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;

            m_animator.SetFloat(_forwardSpeed, speed);
        }
    }
}