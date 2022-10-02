// AIController.cs
// 07-06-2022
// James LaFritz

using System.Linq;
using JetBrains.Annotations;
using RPGEngine.Attributes;
using RPGEngine.Combat;
using RPGEngine.Core;
using Unity.Mathematics;
using UnityEngine;

namespace RPGEngine.Control
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
        private ActionScheduler _actionScheduler;

        #endregion

        #region Optional

        #endregion

        #endregion

        #region Private Fields

        private readonly Collider[] _combatTargetColliders = new Collider[100];

        private Vector3 _startPosition;
        private float _timeSinceLastSawTarget = math.INFINITY;
        private float _timeSinceLastWaypoint = math.INFINITY;

        #endregion

        #region Unity Messages

        #region Overrides of RPGController

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            
            _actionScheduler = GetComponent<ActionScheduler>();
        }

        #endregion

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html"/>
        /// </summary>
        private void Start()
        {
            _startPosition = transform.position;
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            if (Health.IsDead) return;
            if (GamePaused) return;
            
            CombatTarget closestTarget = GetClosestTarget(chaseRange);
            if (IsChasing(closestTarget) && HasFighter && Fighter.CanAttack(closestTarget))
                AttackBehavior(closestTarget!);
            else if (IsLookingForTarget())
                SuspiciousBehavior();
            else
                PatrolBehavior();

            _timeSinceLastSawTarget += Time.deltaTime;
            _timeSinceLastWaypoint += Time.deltaTime;
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
            _timeSinceLastSawTarget = 0;
            Mover.SetMoveSpeed(chaseSpeed);
            if (HasFighter) Fighter.Attack(closestTarget);
        }

        private void SuspiciousBehavior()
        {
            _actionScheduler.CancelCurrentAction();
        }

        private void PatrolBehavior()
        {
            Vector3 nextPosition = _startPosition;
            Mover.SetMoveSpeed(patrolSpeed);

            if (patrolPath)
            {
                if (AtWaypoint())
                    CycleWaypoint();
                nextPosition = GetCurrentWaypoint();
            }

            if (_timeSinceLastWaypoint < waypointTime)
            {
                SuspiciousBehavior();
            }
            else
            {
                Mover.StartMoveAction(nextPosition);
            }
        }

        private bool AtWaypoint()
        {
            return GetTargetDistance(GetCurrentWaypoint()) <= waypointTolerance;
        }

        private void CycleWaypoint()
        {
            if (!patrolPath) return;
            patrolPath.NextIndex();
            _timeSinceLastWaypoint = 0;
        }

        private Vector3 GetCurrentWaypoint()
        {
            return !patrolPath ? _startPosition : patrolPath.GetWaypoint();
        }

        private bool IsChasing(CombatTarget closestTarget)
        {
            return closestTarget;
        }

        private bool IsLookingForTarget()
        {
            return _timeSinceLastSawTarget < lookTime;
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
            Physics.OverlapSphereNonAlloc(transform.position, range, _combatTargetColliders);

            return (from targetCollider in _combatTargetColliders
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