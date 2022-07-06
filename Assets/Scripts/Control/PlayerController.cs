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
    [RequireComponent(typeof(Mover))]
    public class PlayerController : RPGController
    {
        #region Private Fields

        private readonly RaycastHit[] m_combatHits = new RaycastHit[10];

        #endregion

        #region Unity Messages

        #region Overrides of RPGController

        /// <inheritdoc />
        protected override void Update()
        {
            base.Update();
            Debug.Log("Nothing To Do");
        }

        #endregion

        #endregion

        #region Overrides of RPGController

        /// <inheritdoc />
        protected override bool IsInCombat()
        {
            int hits = Physics.RaycastNonAlloc(GetMouseFromMainCameraScreenPointToRay(), m_combatHits);
            if (hits == 0) return false;
            for (int i = 0; i < hits; i++)
            {
                RaycastHit hit = m_combatHits[i];
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (!fighter.CanAttack(target)) continue;
                if (!Input.GetMouseButtonDown(0)) return true;
                Debug.Assert(target != null, nameof(target) + " != null");
                fighter.Attack(target);

                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override bool IsMoving()
        {
            return MoveToCursor();
        }

        #endregion

        #region Private Methods

        private Ray GetMouseFromMainCameraScreenPointToRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private bool MoveToCursor()
        {
            bool hasHIt = Physics.Raycast(GetMouseFromMainCameraScreenPointToRay(), out RaycastHit hit);
            if (!hasHIt || !Input.GetMouseButton(0)) return hasHIt;

            mover.StartMoveAction(hit.point);

            return true;
        }

        #endregion
    }
}