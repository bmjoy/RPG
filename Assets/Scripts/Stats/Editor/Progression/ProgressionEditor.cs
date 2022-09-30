using System.Linq;
using RPGEngine.Stats;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGEditor.Stats
{
    //[CustomEditor(typeof(Progression))]
    public class ProgressionEditor : Editor
    {
        private const int ClassLength = 8;
        private const int StatsLength = 3;
        
        private Progression _progression;

        private SerializedProperty _progressionClasses;
        
        
        /*[MenuItem("Window/RPG Tool Kit/Progression Editor")]
        private static void ShowWindow() => GetWindow<ProgressionEditor>("Stats").Show();*/

        public override VisualElement CreateInspectorGUI()
        {
            _progression = target as Progression;
            CheckProgression();
            _progressionClasses = serializedObject.FindProperty("characterClasses");
            
            VisualElement root = new();
            
            VisualElement weaponStat = new()
            {
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row)
                }
            };
            weaponStat.Add(new Label("Weapon Bonus"));

            FloatField weaponValue = new()
            {
                value = 6
            };
            FloatField weaponPerAdd = new()
            {
                value = 1.25f
            };
            weaponStat.Add(weaponValue);
            weaponStat.Add(weaponPerAdd);
            root.Add(weaponStat);

            VisualElement characterClassesPropertyField = new() 
            {
                name = "characterClasses"
            };

            VisualTreeAsset visualTree = Resources.Load("ProgressionUXML") as VisualTreeAsset;
            if (visualTree != null)
                characterClassesPropertyField = visualTree.Instantiate(_progressionClasses.propertyPath);
            
            root.Add(characterClassesPropertyField);

            return root;
        }

        private void CheckProgression()
        {
            Progression.ProgressionCharacterClass[]
                classes = new Progression.ProgressionCharacterClass[ClassLength];
            var pccIndex = 0;
            if (_progression.CharacterClasses is { Length: ClassLength })
            {
                classes = _progression.CharacterClasses;
                foreach (Progression.ProgressionCharacterClass pcc in classes)
                {
                    classes[pccIndex].Stats = CheckStats(pcc);
                    pccIndex++;
                }

                _progression.CharacterClasses = classes;
                _progression.RebuildLookUpData();
                return;
            }

            var i = 0;
            for (pccIndex = 0; i < ClassLength; i++, pccIndex++)
            {
                CharacterClass cc = (CharacterClass)i;
                Progression.ProgressionCharacterClass progressionCharacterClass =
                    _progression.CharacterClasses.FirstOrDefault(pcc => pcc.CharacterClass == cc);
                if (progressionCharacterClass.Stats is { Length: > 0 })
                    classes[i] = progressionCharacterClass;
                classes[i].CharacterClass = (CharacterClass)i;
                classes[i].Stats = CheckStats(classes[i]);
                classes[i].RebuildLookUpData();
            }

            _progression.CharacterClasses = classes;
            _progression.RebuildLookUpData();
        }
        
        private static Progression.ProgressionStat[] CheckStats(Progression.ProgressionCharacterClass progressionCharacterClass)
        {
            Progression.ProgressionStat[]
                stats = new Progression.ProgressionStat[StatsLength];
            if (progressionCharacterClass.Stats is { Length: StatsLength }) return progressionCharacterClass.Stats;
            Debug.Log($"{progressionCharacterClass.CharacterClass}Updating Stats");
            var indexOffset = 0;
            for (var i = 0; i < StatsLength; i++)
            {
                if (i == 0 && progressionCharacterClass.CharacterClass == CharacterClass.Player)
                {
                    indexOffset ++;
                }

                Stat s = (Stat)indexOffset;
                Progression.ProgressionStat progressionStat = new()
                {
                    Progression = new Progression.ProgressionFormula()
                    {
                        StartingValue = 5, PercentageAdded = 1.25f, AbsoluteAdded = 100, Curve = new AnimationCurve()
                    }
                };
                if (progressionCharacterClass.Stats != null)
                {
                    foreach (Progression.ProgressionStat ps in progressionCharacterClass.Stats)
                    {
                        if (ps.Stat != s) continue;
                        progressionStat = ps;
                        break;
                    }
                }

                stats[i] = progressionStat;
                stats[i].Stat = s;

                if (i == 0 && progressionCharacterClass.CharacterClass != CharacterClass.Player)
                {
                    indexOffset ++;
                }
                indexOffset++;
                Debug.Log($"{progressionCharacterClass.CharacterClass} {indexOffset} = {s}");
            }

            return stats;
        }
    }
}