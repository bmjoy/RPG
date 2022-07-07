// AIController.cs
// 07-06-2022
// James LaFritz

using System.Linq;
using RPG.Attributes;
using RPG.Combat;
using UnityEngine;

namespace RPG.Control
{
    /// <summary>
    /// A <see cref="RPGController"/> that
    /// Controls an Ai Agent in game, Ie Enemy, Player NPC.
    /// </summary>
    public class AIController : RPGController
    {
        #region Inspector Fields

        ///<value>The Range that the Enemy will chase a Combat Target.</value>
        [SerializeField] private float chaseRange = 5f;

        #endregion

        #region Private Fields

        private readonly Collider[] m_combatTargetColliders = new Collider[100];

        #endregion

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            if (health.IsDead) return;
            if (IsChasing()) return;

            if (hasFighter) fighter.Cancel();
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDrawGizmos.html"/>
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get All the Combat Targets that are in range.
        /// </summary>
        /// <returns></returns>
        private CombatTarget[] GetAllCombatTargetsInRange(float range)
        {
            Physics.OverlapSphereNonAlloc(transform.position, range, m_combatTargetColliders);

            return (from targetCollider in m_combatTargetColliders
                    where targetCollider != null
                    select targetCollider.GetComponent<CombatTarget>()
                    into target
                    where target != null
                    where target.Type == combatTargetType
                    let targetHealth = target.GetComponent<Health>()
                    where targetHealth != null
                    where !targetHealth.IsDead
                    select target).OrderBy(GetTargetDistance).ToArray();
        }

        private float GetTargetDistance(CombatTarget target)
        {
            return Vector3.Distance(target.transform.position, transform.position);
        }

        private bool IsChasing()
        {
            CombatTarget[] combatTargets = GetAllCombatTargetsInRange(chaseRange);
            if (combatTargets.Length == 0) return false;
            CombatTarget closestTarget = combatTargets[0];
            if (closestTarget == null || !hasFighter || !fighter.CanAttack(closestTarget)) return false;

            fighter.Attack(closestTarget);
            return true;
        }

        #endregion
    }
}