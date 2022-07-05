// PlayerController.cs
// 07-03-2022
// James LaFritz

using RPG.Combat;
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

        /// <value>Cache the <see cref="Fighter"/></value>
        private Fighter m_fighter;

        bool m_hasFighter;

        #region Unity Methods

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            m_fighter = GetComponent<Fighter>();
            m_hasFighter = m_fighter != null;
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
            InteractWithCombat();
            InteractWithMovement();
        }

        #endregion

        private static Ray GetMouseFromMainCameraScreenPointToRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void InteractWithCombat()
        {
            if (!m_hasFighter) return;
            RaycastHit[] hits = Physics.RaycastAll(GetMouseFromMainCameraScreenPointToRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;
                if (Input.GetMouseButtonDown(0))
                {
                    m_fighter.Attack(target);
                }
            }
        }

        private void InteractWithMovement()
        {
            if (Input.GetMouseButton(0))
            {
                MoveToCursor();
            }
        }

        private void MoveToCursor()
        {
            bool hasHIt = Physics.Raycast(GetMouseFromMainCameraScreenPointToRay(), out RaycastHit hit);
            if (hasHIt)
            {
                m_mover.MoveTo(hit.point);
            }
        }
    }
}