using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGEngine.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "RPG Project/Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [System.Serializable]
        public struct ProgressionFormula
        {
            //[Range(1,1000)]
            [SerializeField] private float startingValue;
            //[Range(0,1)]
            [SerializeField] private float percentageAdd;
            //[Range(0,1000)]
            [SerializeField] private float absoluteAdd;

            [SerializeField] private bool useCurve;
            [SerializeField] private  AnimationCurve curve;

            public float StartingValue
            {
                get => startingValue;
                set
                {
#if UNITY_EDITOR
                    startingValue = value;
#endif
                }
            }
            public float PercentageAdded
            {
                get => percentageAdd;
                set
                {
#if UNITY_EDITOR
                    percentageAdd = value;
#endif
                }
            }
            public float AbsoluteAdded
            {
                get => absoluteAdd;
                set
                {
#if UNITY_EDITOR
                    absoluteAdd = value;
#endif
                }
            }
            public bool UseCurve
            {
                get => useCurve;
                set
                {
#if UNITY_EDITOR
                    useCurve = value;
#endif
                }
            }
            public AnimationCurve Curve
            {
                get => curve;
                set
                {
#if UNITY_EDITOR
                    curve = value;
#endif
                }
            }

            public float Calculate(int level)
            {
                if (level <= 1) return startingValue;
                var c = Calculate(level - 1);
                var value = c + c * percentageAdd + absoluteAdd;
                if (!useCurve) return value;
                return value + c * curve.Evaluate(1f / level);
            }
        }

        [System.Serializable]
        public struct ProgressionStat
        {
            [SerializeField] private Stat stat;
            [SerializeField] private ProgressionFormula progression;

            public Stat Stat
            {
                get => stat;
                set
                {
                    #if UNITY_EDITOR
                    stat = value;
                    #endif
                }
            }

            public ProgressionFormula Progression
            {
                get => progression;
                set
                {
                    #if UNITY_EDITOR
                    progression = value;
                    #endif
                }
            }
        }
        
        [System.Serializable]
        public struct ProgressionCharacterClass
        {
            [SerializeField] private CharacterClass characterClass;
            [SerializeField] private ProgressionStat[] stats;
            
            private Dictionary<Stat, ProgressionFormula> _lookupTable;

            public CharacterClass CharacterClass
            {
                get => characterClass;
                #if UNITY_EDITOR
                set => characterClass = value;
                #endif
            }

            public ProgressionStat[] Stats
            { 
                get
                {
                    #if UNITY_EDITOR
                    return stats;
                    #endif
                }
                set
                {
                    #if UNITY_EDITOR
                    stats = value;
                    #endif
                }
            }

            public ProgressionFormula this[Stat stat] => FindStat(stat);

            public void RebuildLookUpData()
            {
#if !UNITY_EDITOR
                return;
#endif
                _lookupTable = null;
                BuildLookupTable();
            }

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

        public ProgressionCharacterClass[] CharacterClasses
        {
            get
            {
                #if UNITY_EDITOR
                return characterClasses;
                #endif
            }
            set
            {
                #if UNITY_EDITOR
                characterClasses = value;
                #endif
            }
        }

        private Dictionary<CharacterClass, ProgressionCharacterClass> _lookupTable;

        public ProgressionFormula this[CharacterClass cc, Stat stat] => FindProgressionCharacterClassWithStat(cc, stat);

        public void RebuildLookUpData()
        {
#if !UNITY_EDITOR
                return;
#endif
            _lookupTable = null;
            BuildLookupTable();
        }

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