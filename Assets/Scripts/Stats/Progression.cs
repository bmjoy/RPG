using RPGEngine.Core;
using UnityEngine;

namespace RPGEngine.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "RPG Project/Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [System.Serializable]
        public struct ProgressionFormula
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
        private struct ProgressionStat
        {
            [SerializeField] private Stat stat;
            [SerializeField] private ProgressionFormula progression;
            public Stat Stat => stat;
            public ProgressionFormula Progression => progression;
        }
        
        [System.Serializable]
        private struct ProgressionCharacterClass
        {
            [SerializeField] private CharacterClass characterClass;
            [SerializeField] private ProgressionStat[] stats;

            public CharacterClass CharacterClass => characterClass;
            public ProgressionFormula this[Stat stat] => FindStat(stat);

            private ProgressionFormula FindStat(Stat stat)
            {
                foreach (ProgressionStat s in stats)
                {
                    if (s.Stat == stat) return s.Progression;
                }

                throw new System.ArgumentOutOfRangeException(
                    nameof(stat), $"Progression Character Class does not contain a Stat of {stat}");
            }
        }
        
        [SerializeField] private ProgressionCharacterClass[] characterClasses;
        public ProgressionFormula this[CharacterClass cc, Stat stat] => FindProgressionCharacterClassWithStat(cc, stat);

        private ProgressionFormula FindProgressionCharacterClassWithStat(CharacterClass cc, Stat stat)
        {
            var ccFound = false;
            var error = "";
            foreach (ProgressionCharacterClass pcc in characterClasses)
            {
                if (pcc.CharacterClass != cc) continue;
                try
                {
                    return pcc[stat];
                }
                catch (System.ArgumentOutOfRangeException e)
                {
                    error =
                        $"{(ccFound ? $"Multiple progressions for {cc} found:\n{error}\n" : "")}No {stat} found in {cc}\n{e}";
                }
                    
                ccFound = true;
            }
            
            if (!ccFound)
                throw new System.ArgumentOutOfRangeException(
                    nameof(characterClasses), $"No Progression {cc} found");

            throw new System.ArgumentOutOfRangeException(nameof(characterClasses), error);
        }
    }
}