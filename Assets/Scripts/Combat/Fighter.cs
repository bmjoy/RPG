// Fighter.cs
// 07-03-2022
// James LaFritz

using JetBrains.Annotations;
using RPGEngine.Attributes;
using RPGEngine.Core;
using RPGEngine.Movement;
using Unity.Mathematics;
using UnityEngine;
using static RPGEngine.Core.StringReferences;

namespace RPGEngine.Combat
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html">UnityEngine.MonoBehavior</a> that
    /// is a Fighter in the game.
    /// Once the target is set by the attack method it will move towards the target and attack it.
    /// <p>
    /// Implements
    /// <see cref="IAction"/>
    /// </p>
    /// <p>
    /// <a href="https://docs.unity3d.com/ScriptReference/RequireComponent.html">UnityEngine.RequireComponent</a>(
    /// typeof(<see cref="Mover"/>)
    /// , typeof(<see cref="ActionScheduler"/>)
    /// )</p>
    /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.html"/>
    /// </summary>
    [RequireComponent(typeof(Mover), typeof(ActionScheduler))]
    public class Fighter : MonoBehaviour, IAction
    {
        #region Inspector Fields

        [Header("Weapons")]
        [SerializeField]
        private Weapon defaultWeapon;

        private bool _hasWeapon;

        [Header("Weapon Slots")]
        [SerializeField]
        private Transform rightHandWeaponSlot;
        [SerializeField] private Transform leftHandWeaponSlot;

        [Header("Targeting")]
        [SerializeField]
        private CombatTargetType combatTargetType = CombatTargetType.Player;

        #endregion

        #region Private Fields

        private Health _target;
        private bool _hasTarget;

        private float _timeSinceLastAttack = math.INFINITY;
        
        private Weapon _currentWeapon;

        #endregion

        #region Component References

        #region Required

        /// <value>Cache the <see cref="Mover"/></value>
        private Mover _mover;

        /// <value>Cache the <see cref="ActionScheduler"/></value>
        private ActionScheduler _actionScheduler;

        #endregion

        #region Optional

        /// <value>Cache the <a href="https://docs.unity3d.com/ScriptReference/Animator.html">UnityEngine.Animator</a></value>
        private Animator _animator;

        private bool _hasAnimator;
        private static int _attackHash;
        private static int _stopAttackHash;

        #endregion

        #endregion

        #region Unity Messages

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html"/>
        /// </summary>
        private void Awake()
        {
            _mover = GetComponent<Mover>();
            _actionScheduler = GetComponent<ActionScheduler>();

            _animator = GetComponentInChildren<Animator>();
            _hasAnimator = _animator != null;

            if (_hasAnimator)
            {
                _attackHash = Animator.StringToHash(attackTrigger);
                _stopAttackHash = Animator.StringToHash(stopAttackTrigger);
            }

            string errorObject = "";
            if (_mover == null) errorObject = nameof(_mover);
            if (_actionScheduler == null) errorObject += nameof(_actionScheduler);
            if (string.IsNullOrEmpty(errorObject)) return;

            // ReSharper disable Unity.InefficientPropertyAccess
            Debug.LogError($"{gameObject.name} requires a(n) {errorObject} in order to work", gameObject);
            enabled = false;
            // ReSharper restore Unity.InefficientPropertyAccess
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html"/>
        /// </summary>
        private void Start()
        {
            EquipWeapon(defaultWeapon);
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;

            if (!_hasTarget) return;

            if (_target.IsDead)
            {
                Cancel();
                return;
            }

            if (!GetIsInRange(_target.transform))
            {
                _mover.MoveTo(_target.transform.position);
            }
            else
            {
                _mover.Cancel();
                AttackBehavior();
            }
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDrawGizmos.html"/>
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var weaponRange = _hasWeapon ? _currentWeapon.Range : defaultWeapon != null ? defaultWeapon.Range : 0;
            Gizmos.DrawWireSphere(transform.position, weaponRange);
        }

        #endregion

        #region Implementation of IAction

        /// <inheritdoc />
        public void Cancel()
        {
            _hasTarget = false;
            _target = null;
            _mover.Cancel();
            TriggerCancelAttack();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attack a target.
        /// </summary>
        /// <param name="target">The <see cref="CombatTarget"/> to attack.</param>
        public void Attack([NotNull] CombatTarget target)
        {
            _actionScheduler.StartAction(this);
            _target = target.GetComponent<Health>();
            _hasTarget = _target;
        }

        public bool CanAttack(CombatTarget combatTarget)
        {
            if (!combatTarget) return false;
            if (combatTarget.Type != combatTargetType) return false;
            Health targetHealth = combatTarget.GetComponent<Health>();
            return targetHealth && !targetHealth.IsDead;
        }

        #endregion

        #region Private Methods

        private bool GetIsInRange(Transform targetTransform)
        {
            return targetTransform && _hasWeapon &&
                   Vector3.Distance(transform.position, targetTransform.position) < _currentWeapon.Range;
        }

        private void AttackBehavior()
        {
            if (!_hasTarget || !_hasWeapon) return;

            if (_timeSinceLastAttack < _currentWeapon.TimeBetweenAttacks) return;
            transform.LookAt(_target.transform);
            TriggerAttack();
        }

        private void TriggerAttack()
        {
            if (_hasAnimator)
            {
                _animator.ResetTrigger(_stopAttackHash);
                // This will trigger Hit() from the Animation Event.
                _animator.SetTrigger(_attackHash);
            }
            else
            {
                Hit();
            }
        }

        private void TriggerCancelAttack()
        {
            if (!_hasAnimator) return;
            _animator.ResetTrigger(_attackHash);
            _animator.SetTrigger(_stopAttackHash);
        }

        /// <summary>
        /// Need for Animation Event.
        /// Will Damage the target in sync with the animation.
        /// </summary>
        private void Hit()
        {
            if (!_hasTarget || !_hasWeapon) return;

            if (_currentWeapon.HasProjectile())
                _currentWeapon.LaunchProjectile(rightHandWeaponSlot, leftHandWeaponSlot, _target);
            else 
                _target.TakeDamage(_currentWeapon.Damage);
        }

        private void Shoot()
        {
            Hit();
        }

        public void EquipWeapon(Weapon weapon)
        {
            _hasWeapon = weapon != null;
            if (!_hasWeapon) return;
            _currentWeapon = weapon;
            weapon.Spawn(rightHandWeaponSlot, leftHandWeaponSlot,  _animator);
        }

        #endregion
    }
}