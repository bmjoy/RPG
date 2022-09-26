using System.Collections.Generic;
using System.Linq;
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
            
            private Dictionary<Stat, ProgressionFormula> _lookupTable;

            public CharacterClass CharacterClass => characterClass;
            public ProgressionFormula this[Stat stat] => FindStat(stat);

            private ProgressionFormula FindStat(Stat stat)
            {
                BuildLookupTable();

                if (!_lookupTable.ContainsKey(stat))
                    throw new System.ArgumentOutOfRangeException(nameof(stats),
                        $"Progression does not contain an entry for {stat} in the class {characterClass}");

                return _lookupTable[stat];
            }

            private void BuildLookupTable()
            {
                if (_lookupTable != null) return;
                _lookupTable = stats.ToDictionary(pStat => pStat.Stat, pStat => pStat.Progression);
            }
        }
        
        [SerializeField] private ProgressionCharacterClass[] characterClasses;

        private Dictionary<CharacterClass, ProgressionCharacterClass> _lookupTable;
        public ProgressionFormula this[CharacterClass cc, Stat stat] => FindProgressionCharacterClassWithStat(cc, stat);

        private ProgressionFormula FindProgressionCharacterClassWithStat(CharacterClass cc, Stat stat)
        {
            try
            {
                BuildLookupTable();
                return _lookupTable[cc][stat];
            }
            catch (System.ArgumentOutOfRangeException)
            {
                throw;
            }
            catch (System.Exception e)
            {
                if(!_lookupTable.ContainsKey(cc))
                {
                    throw new System.ArgumentOutOfRangeException(nameof(characterClasses),
                        $"{name} does not contain an entry for characterClass {cc}\n{e}");
                }

                throw new System.Exception($"{name}: Something went Horribly wrong!\n{e}");
            }
        }

        private void BuildLookupTable()
        {
            if (_lookupTable != null) return;

            _lookupTable = characterClasses.ToDictionary(pcc => pcc.CharacterClass,
                pcc => pcc);
        }
    }
}