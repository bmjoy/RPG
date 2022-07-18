// Weapon.cs
// 07-18-2022
// James LaFritz

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
        [SerializeField] GameObject weaponPrefab;

        [Header("Weapon Attributes")] [SerializeField]
        private float range = 2f;

        [SerializeField] private float timeBetweenAttacks = 1f;

        [SerializeField] private float damage = 10f;

        [SerializeField] private AnimatorOverrideController animatorOverrideController;

        public float Range => range;
        public float TimeBetweenAttacks => timeBetweenAttacks;
        public float Damage => damage;

        public void Spawn(Transform handTransform, Animator animator)
        {
            if (weaponPrefab != null)
                Instantiate(weaponPrefab, handTransform);

            if (animator == null || animatorOverrideController == null) return;
            animator.runtimeAnimatorController = animatorOverrideController;
        }
    }
}