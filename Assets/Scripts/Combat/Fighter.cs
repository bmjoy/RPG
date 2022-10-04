// Fighter.cs
// 07-03-2022
// James LaFritz

using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using RPGEngine.Attributes;
using RPGEngine.Core;
using RPGEngine.Movement;
using RPGEngine.Saving;
using RPGEngine.Stats;
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
    [RequireComponent(typeof(Mover), typeof(ActionScheduler), typeof(BaseStats))]
    public class Fighter : MonoBehaviour, IAction, ISavable, IModifierProvider
    {
        #region Inspector Fields
        
        [SerializeField] private BoolGameEvent gamePaused;

        [Header("Weapons")]
        [SerializeField]
        private WeaponConfig defaultWeaponConfig;

        private bool _hasWeaponConfig;

        [Header("Weapon Slots")]
        [SerializeField]
        private Transform rightHandWeaponSlot;
        [SerializeField] private Transform leftHandWeaponSlot;

        [Header("Targeting")]
        [SerializeField]
        private CombatTargetType combatTargetType = CombatTargetType.Player;

        [SerializeField] private GameObjectGameEvent onTargetChanged;
        [SerializeField] private GameObjectFloatGameEvent dealDamage;

        #endregion

        #region Private Fields

        private Health _target;

        private float _timeSinceLastAttack;
        
        private WeaponConfig _currentWeaponConfig;

        private Weapon _currentWeapon;

        private bool _gamePaused;

        #endregion

        #region Component References

        #region Required

        /// <value>Cache the <see cref="Mover"/></value>
        private Mover _mover;

        /// <value>Cache the <see cref="ActionScheduler"/></value>
        private ActionScheduler _actionScheduler;

        private BaseStats _baseStats;

        #endregion

        #region Optional

        /// <value>Cache the <a href="https://docs.unity3d.com/ScriptReference/Animator.html">UnityEngine.Animator</a></value>
        private Animator _animator;

        private bool _hasAnimator;
        private static int _attackHash;
        private static int _stopAttackHash;
        private bool _hasWeapon;

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
            _baseStats = GetComponent<BaseStats>();

            _animator = GetComponentInChildren<Animator>();
            _hasAnimator = _animator != null;

            if (_hasAnimator)
            {
                _attackHash = Animator.StringToHash(AttackTrigger);
                _stopAttackHash = Animator.StringToHash(StopAttackTrigger);
            }

            EquipWeapon(defaultWeaponConfig);

            var errorObject = "";
            if (_mover == null) errorObject = nameof(_mover);
            if (_actionScheduler == null) errorObject += nameof(_actionScheduler);
            if (string.IsNullOrEmpty(errorObject)) return;

            // ReSharper disable Unity.InefficientPropertyAccess
            Debug.LogError($"{gameObject.name} requires a(n) {errorObject} in order to work", gameObject);
            enabled = false;
            // ReSharper restore Unity.InefficientPropertyAccess
        }

        private void OnEnable()
        {
            if(gamePaused) gamePaused.RegisterListener(OnGamePaused);
        }

        private void OnDisable()
        {
            if(gamePaused) gamePaused.UnregisterListener(OnGamePaused);
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html"/>
        /// </summary>
        private void Update()
        {
            if (_gamePaused) return;
            //if (name != "Player") return;
            _timeSinceLastAttack += Time.deltaTime;

            if (!_target) return;
            //Debug.Log($"{name} has Target");

            if (_target.IsDead)
            {
                Cancel();
                return;
            }
            
            //Debug.Log($"{_target.name} Is Alive");

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
            var weaponRange = _hasWeaponConfig ? _currentWeaponConfig.Range : defaultWeaponConfig != null ? defaultWeaponConfig.Range : 0;
            Gizmos.DrawWireSphere(transform.position, weaponRange);
        }

        #endregion
        
        #region Implementation of IJsonSavable

        /// <inheritdoc />
        public JToken CaptureAsJToken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            stateDict["Current Weapon"] = _currentWeaponConfig.name;
            return state;
        }

        /// <inheritdoc />
        public void RestoreFromJToken(JToken state, int version)
        {
            if (state == null || version < 6) return;

            IDictionary<string, JToken> stateDict = state.ToObject<IDictionary<string, JToken>>();
            if (stateDict == null || stateDict.Count < 1) return;
            if (stateDict.ContainsKey("Current Weapon"))
                EquipWeapon(Resources.Load<WeaponConfig>(stateDict["Current Weapon"].ToString()));
                
        }

        #endregion

        #region Implementation of IAction

        /// <inheritdoc />
        public void Cancel()
        {
            ChangeTarget(null);
            _mover.Cancel();
            TriggerCancelAttack();
        }

        #endregion

        #region Implementation of IModifierProvider

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.Damage;
            }
        }
        
        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.DamagePercentageBonus;
            }
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
            ChangeTarget(target);
        }

        public bool CanAttack(CombatTarget combatTarget)
        {
            if (!combatTarget) return false;
            if (combatTarget.Type != combatTargetType) return false;
            if (!_mover.CanMoveTo(combatTarget.transform.position)) return false;
            Health targetHealth = combatTarget.GetComponent<Health>();
            return targetHealth && !targetHealth.IsDead;
        }

        #endregion

        #region Private Methods

        private void ChangeTarget(CombatTarget target)
        {
            if (!target)
            {
                //Debug.Log("Target is null");
                _target = null;
                if (onTargetChanged) onTargetChanged.Invoke(null);
                return;
            }

            _target = target.GetComponent<Health>();
            if (!_target)
            {
                //Debug.Log($"{target.name} has no health");
                _target = null;
                if (onTargetChanged) onTargetChanged.Invoke(null);
                return;
            }
            // if (name == "Player")
            //         Debug.Log($"{name} is targeting {target.name}");
            if (onTargetChanged) onTargetChanged.Invoke(_target.gameObject);
            _target.OnHealthChange();
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return targetTransform && _hasWeaponConfig &&
                   Vector3.Distance(transform.position, targetTransform.position) < _currentWeaponConfig.Range;
        }

        private void AttackBehavior()
        {
            if (!_target || !_hasWeaponConfig) return;

            if (_timeSinceLastAttack < _currentWeaponConfig.TimeBetweenAttacks) return;
            transform.LookAt(_target.transform);
            TriggerAttack();
            _timeSinceLastAttack = Time.deltaTime;
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
            if (!_target || !_hasWeaponConfig) return;

            var damageAmount = _baseStats.GetStatValue(Stat.Damage);
            
            if (_hasWeapon) _currentWeapon.OnHit();

            if (_currentWeaponConfig.HasProjectile())
                _currentWeaponConfig.LaunchProjectile(rightHandWeaponSlot, leftHandWeaponSlot, _target, dealDamage,
                    damageAmount, tag, gamePaused, _gamePaused);
            else if (dealDamage) dealDamage.Invoke(_target.gameObject, damageAmount);
        }

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            if (!weaponConfig) return;
            _currentWeaponConfig = weaponConfig;
            _hasWeaponConfig = true;
            _currentWeapon = weaponConfig.Spawn(rightHandWeaponSlot, leftHandWeaponSlot,  _animator);
            _hasWeapon = _currentWeapon;
        }

        private void OnGamePaused(bool paused)
        {
            _gamePaused = paused;
        }

        #endregion
    }
}