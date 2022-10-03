// Weapon.cs
// 07-18-2022
// James LaFritz

using RPGEngine.Attributes;
using RPGEngine.Core;
using UnityEngine;

namespace RPGEngine.Combat
{
    /// <summary>
    /// Weapon Data
    /// <seealso href="https://docs.unity3d.com/ScriptReference/ScriptableObject.html"/>
    /// </summary>
    [CreateAssetMenu(fileName = "Weapon", menuName = "RPG Project/Items/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        private const string WeaponName = "Weapon";

        [SerializeField] private Weapon equippedPrefab;
        [SerializeField] private AnimatorOverrideController animatorOverrideController;
        [SerializeField] private bool isWeaponRightHanded = true;

        [Header("Weapon Attributes")] [SerializeField]
        private float range = 2f;
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float damagePercentageBonus;

        [Header("Weapon Projectile")] [SerializeField]
        private Projectile projectile;

        [SerializeField] private bool isProjectileRightHanded = true;

        public float Range => range;
        public float TimeBetweenAttacks => timeBetweenAttacks;
        public float Damage => damage;
        public float DamagePercentageBonus => damagePercentageBonus;

        public Weapon Spawn(Transform rightHand, Transform leftHand,  Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            Weapon weapon = null;
            if (equippedPrefab)
            {
                weapon = Instantiate(equippedPrefab, GetParent(rightHand, leftHand, isWeaponRightHanded));
                weapon.gameObject.name = WeaponName;
            }

            if (!animator) return weapon;
            if (animatorOverrideController) animator.runtimeAnimatorController = animatorOverrideController;
            else if (animator.runtimeAnimatorController is AnimatorOverrideController overrideController)
            { 
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target,
            GameObjectFloatGameEvent dealDamage, float calculatedDamage, string tag, BoolGameEvent gamePausedEvent,
            bool isPaused)
        {
            Projectile projectileInstance =
                Instantiate(projectile, GetParent(rightHand, leftHand, isProjectileRightHanded).position,
                    Quaternion.identity);
            projectileInstance.tag = tag;
            projectileInstance.SetTarget(target, dealDamage, calculatedDamage, gamePausedEvent, isPaused);
        }

        public bool HasProjectile()
        {
            return projectile;
        }

        private Transform GetParent(Transform rightHand, Transform leftHand, bool isRightHanded)
        {
            return isRightHanded ? rightHand : leftHand;
        }

        private static void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = null;
            if (rightHand) oldWeapon = rightHand.Find(WeaponName);
            if (!oldWeapon && leftHand) oldWeapon = leftHand.Find(WeaponName);
            if (!oldWeapon) return;
            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }
    }
}