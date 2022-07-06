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
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// typeof(<see cref="Mover"/>)
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(Mover))]
    public class PlayerController : MonoBehaviour
    {
        #region Private Fields

        private RaycastHit[] m_combatHits = new RaycastHit[10];

        #endregion

        #region Component References

        #region Required

        /// <value>Cache the <see cref="Mover"/></value>
        private Mover m_mover;

        #endregion

        #region Optional

        /// <value>Cache the <see cref="Fighter"/></value>
        private Fighter m_fighter;

        bool m_hasFighter;

        #endregion

        #endregion

        #region Unity Messages

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
            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
            Debug.Log("Nothing To Do");
        }

        #endregion

        #region Private Methods

        private Ray GetMouseFromMainCameraScreenPointToRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private bool InteractWithCombat()
        {
            if (!m_hasFighter) return false;
            int hits = Physics.RaycastNonAlloc(GetMouseFromMainCameraScreenPointToRay(), m_combatHits);
            if (hits == 0) return false;
            for (int i = 0; i < hits; i++)
            {
                RaycastHit hit = m_combatHits[i];
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (!m_fighter.CanAttack(target)) continue;
                if (Input.GetMouseButtonDown(0))
                {
                    m_fighter.Attack(target);
                }

                return true;
            }

            return false;
        }

        private bool InteractWithMovement()
        {
            return MoveToCursor();
        }

        private bool MoveToCursor()
        {
            bool hasHIt = Physics.Raycast(GetMouseFromMainCameraScreenPointToRay(), out RaycastHit hit);
            if (!hasHIt || !Input.GetMouseButton(0)) return hasHIt;

            m_mover.StartMoveAction(hit.point);

            return true;
        }

        #endregion
    }
}