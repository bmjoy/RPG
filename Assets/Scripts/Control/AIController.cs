// AIController.cs
// 07-06-2022
// James LaFritz

using System.Linq;
using JetBrains.Annotations;
using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using Unity.Mathematics;
using UnityEngine;

namespace RPG.Control
{
    /// <summary>
    /// A <see cref="RPGController"/> that
    /// Controls an Ai Agent in game, Ie Enemy, Player NPC.
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// typeof(<see cref="ActionScheduler"/>)
    /// )</p>
    /// </summary>
    [RequireComponent(typeof(ActionScheduler))]
    public class AIController : RPGController
    {
        #region Inspector Fields

        ///<value>The Range that the Enemy will chase a Combat Target.</value>
        [SerializeField] private float chaseRange = 5f;

        ///<value>The Speed to move when chasing.</value>
        [SerializeField] private float chaseSpeed = 2f;

        ///<value>The amount of time to look for target after target has been lost.</value>
        [SerializeField] private float lookTime = 5f;

        ///<value>The <see cref="patrolPath"/> to patrol.</value>
        [SerializeField] private PatrolPath patrolPath;

        ///<value>The Speed to walk when patrolling.</value>
        [SerializeField] private float patrolSpeed = 1.508f;

        ///<value>How far from the way point the ai needs to be before moving to the next waypoint.</value>
        [SerializeField] private float waypointTolerance = 1f;

        //<value>The amount of time to wait before moving to the next way point.</value>
        [SerializeField] private float waypointTime = 6f;

        #endregion

        #region Component References

        #region Required

        /// <value>Cache the <see cref="ActionScheduler"/></value>
        private ActionScheduler m_actionScheduler;

        #endregion

        #region Optional

        #endregion

        #endregion

        #region Private Fields

        private readonly Collider[] m_combatTargetColliders = new Collider[100];

        private Vector3 m_startPosition;
        private float m_timeSinceLastSawTarget = math.INFINITY;
        private float m_timeSinceLastWaypoint = math.INFINITY;

        #endregion

        #region Unity Messages

        #region Overrides of RPGController

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            m_actionScheduler = GetComponent<ActionScheduler>();
        }

        #endregion

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html"/>
        /// </summary>
        private void Start()
        {
            m_startPosition = transform.position;
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            if (health.IsDead) return;
            CombatTarget closestTarget = GetClosestTarget(chaseRange);
            if (IsChasing(closestTarget) && hasFighter && fighter.CanAttack(closestTarget))
                AttackBehavior(closestTarget!);
            else if (IsLookingForTarget())
                SuspiciousBehavior();
            else
                PatrolBehavior();

            m_timeSinceLastSawTarget += Time.deltaTime;
            m_timeSinceLastWaypoint += Time.deltaTime;
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

        #region Behavior Methods

        private void AttackBehavior([NotNull] CombatTarget closestTarget)
        {
            m_timeSinceLastSawTarget = 0;
            mover.SetMoveSpeed(chaseSpeed);
            fighter.Attack(closestTarget);
        }

        private void SuspiciousBehavior()
        {
            m_actionScheduler.CancelCurrentAction();
        }

        private void PatrolBehavior()
        {
            Vector3 nextPosition = m_startPosition;
            mover.SetMoveSpeed(patrolSpeed);

            if (patrolPath != null)
            {
                if (AtWaypoint())
                    CycleWaypoint();
                nextPosition = GetCurrentWaypoint();
            }

            if (m_timeSinceLastWaypoint < waypointTime)
            {
                SuspiciousBehavior();
            }
            else
            {
                mover.StartMoveAction(nextPosition);
            }
        }

        private bool AtWaypoint()
        {
            return GetTargetDistance(GetCurrentWaypoint()) <= waypointTolerance;
        }

        private void CycleWaypoint()
        {
            if (patrolPath == null) return;
            patrolPath.NextIndex();
            m_timeSinceLastWaypoint = 0;
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath == null ? m_startPosition : patrolPath.GetWaypoint();
        }

        private bool IsChasing(CombatTarget closestTarget)
        {
            return closestTarget != null;
        }

        private bool IsLookingForTarget()
        {
            return m_timeSinceLastSawTarget < lookTime;
        }

        #endregion

        #region Private Methods

        private CombatTarget GetClosestTarget(float range)
        {
            CombatTarget[] combatTargets = GetAllCombatTargetsInRange(range);
            if (combatTargets.Length == 0) return null;
            CombatTarget closestTarget = combatTargets[0];
            return closestTarget;
        }

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
                    select target).OrderBy(d => GetTargetDistance(d.transform.position)).ToArray();
        }

        private float GetTargetDistance(Vector3 target)
        {
            return Vector3.Distance(target, transform.position);
        }

        #endregion
    }
}