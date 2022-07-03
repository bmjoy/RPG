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

        /// <value>Cache the last <a href="https://docs.unity3d.com/ScriptReference/Ray.html">UnityEngine.Ray</a> that was created.</value>
        private Ray m_lastRay;

        #region Unity Methods

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
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
            if (Input.GetMouseButtonDown(0))
            {
                m_lastRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            }

            // Draw the last Ray.
            Debug.DrawRay(m_lastRay.origin, m_lastRay.direction * 100, Color.red);

            // If the target is null, return.
            if (target == null) return;

            // Set the nav mesh agent's destination to the target's position
            m_navMeshAgent!.destination = target.position;
        }

        #endregion
    }
}