// PlayerController.cs
// 07-03-2022
// James LaFritz

using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// Controls a Player in game.
    /// to move a game object to a targets position.
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent(</a>
    /// <see cref="Mover"/>
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(Mover))]
    public class PlayerController : MonoBehaviour
    {
        /// <value>Cache the <see cref="Mover"/></value>
        private Mover m_mover;

        #region Unity Methods

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            m_mover = GetComponent<Mover>();
            // ReSharper disable Unity.InefficientPropertyAccess
            if (m_mover != null) return;
            Debug.LogError($"{gameObject.name} requires a(n) {nameof(m_mover)} in order to work", gameObject);
            enabled = false;
            // ReSharper restore Unity.InefficientPropertyAccess
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                MoveToCursor();
            }
        }

        #endregion

        private void MoveToCursor()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hasHIt = Physics.Raycast(ray, out RaycastHit hit);
            if (hasHIt)
            {
                m_mover.MoveTo(hit.point);
            }
        }
    }
}