using System.Linq;
using RPGEngine.Core;
using UnityEngine;

namespace RPGEngine.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "RPG Project/Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [System.Serializable]
        private struct ProgressionFormula
        {
            [Range(1,1000)]
            [SerializeField] private float startingValue;
            [Range(0,1)]
            [SerializeField] private float percentageAdded;
            [Range(0,1000)]
            [SerializeField] private float absoluteAdded;

            [SerializeField] private bool useCurve;
            [SerializeField, ShowIfBool("useCurve")] private  AnimationCurve curve;

            public float Calculate(int level)
            {
                if (level <= 1) return startingValue;
                var c = Calculate(level - 1);
                var value = c + c * percentageAdded + absoluteAdded;
                if (!useCurve) return value;
                return value + c * curve.Evaluate(1f / level);
            }
        }
        
        [System.Serializable]
        private struct ProgressionCharacterClass
        {
            [SerializeField] private CharacterClass characterClass;
            [SerializeField] private ProgressionFormula experienceToLevel;
            [SerializeField] private ProgressionFormula health;
            [SerializeField] private ProgressionFormula damage;

            public float GetExperienceToLevel(CharacterClass characterClassToCheck, int level)
            {
                return characterClass == characterClassToCheck ? experienceToLevel.Calculate(level) : float.NegativeInfinity;
            }
            public float GetHealth(CharacterClass characterClassToCheck, int level)
            {
                return characterClass == characterClassToCheck ? health.Calculate(level) : float.NegativeInfinity;
            }
            
            public float GetDamage(CharacterClass characterClassToCheck, int level)
            {
                return characterClass == characterClassToCheck ? damage.Calculate(level) : float.NegativeInfinity;
            }
        }
        
        [SerializeField] private ProgressionCharacterClass[] characterClasses;

        
        public float GetExperienceToLevel(CharacterClass characterClass, int level)
        {
            if (characterClasses == null) return 0;

            return characterClasses.Select(progression => progression.GetExperienceToLevel(characterClass, level))
                .FirstOrDefault(value => !float.IsNegativeInfinity(value));
        }
        
        public float GetHealth(CharacterClass characterClass, int level)
        {
            if (characterClasses == null) return 0;

            return characterClasses.Select(progression => progression.GetHealth(characterClass, level))
                .FirstOrDefault(value => !float.IsNegativeInfinity(value));
        }
        
        public float GetDamage(CharacterClass characterClass, int level)
        {
            if (characterClasses == null) return 0;

            return characterClasses.Select(progression => progression.GetDamage(characterClass, level))
                .FirstOrDefault(value => !float.IsNegativeInfinity(value));
        }
    }
}