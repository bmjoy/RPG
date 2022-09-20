// Weapon.cs
// 07-18-2022
// James LaFritz

using RPGEngine.Attributes;
using UnityEngine;

namespace RPGEngine.Combat
{
    /// <summary>
    /// Weapon Data
    /// <seealso href="https://docs.unity3d.com/ScriptReference/ScriptableObject.html"/>
    /// </summary>
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] private GameObject equippedPrefab;
        [SerializeField] private AnimatorOverrideController animatorOverrideController;
        [SerializeField] private bool isWeaponRightHanded = true;

        [Header("Weapon Attributes")] [SerializeField]
        private float range = 2f;
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] private float damage = 10f;

        [Header("Weapon Projectile")] [SerializeField]
        private Projectile projectile;
        [SerializeField] private bool isProjectileRightHanded = true;

        public float Range => range;
        public float TimeBetweenAttacks => timeBetweenAttacks;
        public float Damage => damage;

        public void Spawn(Transform rightHand, Transform leftHand,  Animator animator)
        {
            if (equippedPrefab != null)
                Instantiate(equippedPrefab, GetParent(rightHand, leftHand, isWeaponRightHanded));

            if (animator == null || animatorOverrideController == null) return;
            animator.runtimeAnimatorController = animatorOverrideController;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
        {
            Projectile projectileInstance =
                Instantiate(projectile, GetParent(rightHand, leftHand, isProjectileRightHanded).position, Quaternion.identity);
            projectileInstance.SetTarget(target, damage);
        }

        public bool HasProjectile()
        {
            return projectile;
        }

        private Transform GetParent(Transform rightHand, Transform leftHand, bool isRightHanded)
        {
            return isRightHanded ? rightHand : leftHand;
        }
    }
}