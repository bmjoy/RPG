// PlayerController.cs
// 07-03-2022
// James LaFritz

using RPG.Combat;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    /// <summary>
    /// A <see cref="RPGController"/> that
    /// Controls a User Controlled Player in Game.
    /// </summary>
    public class PlayerController : RPGController
    {
        #region Private Fields

        private readonly RaycastHit[] m_combatHits = new RaycastHit[10];

        #endregion

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            if(health.IsDead) return;
            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
            Debug.Log("Nothing To Do");
        }

        #endregion

        #region Private Methods

        private bool InteractWithCombat()
        {
            if (!hasFighter) return false;
            int hits = Physics.RaycastNonAlloc(GetMouseFromMainCameraScreenPointToRay(), m_combatHits);
            if (hits == 0) return false;
            for (int i = 0; i < hits; i++)
            {
                RaycastHit hit = m_combatHits[i];
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (!fighter.CanAttack(target)) continue;
                if (!Input.GetMouseButton(0)) return true;
                Debug.Assert(target != null, nameof(target) + " != null");
                fighter.Attack(target);

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

            mover.StartMoveAction(hit.point);

            return true;
        }

        private Ray GetMouseFromMainCameraScreenPointToRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        #endregion
    }
}