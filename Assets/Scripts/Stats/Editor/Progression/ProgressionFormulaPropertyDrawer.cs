using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGEditor.Stats
{
    //[CustomPropertyDrawer(typeof(Progression.ProgressionFormula))]
    public class ProgressionFormulaPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement progressionFormulaElement = new();
            VisualTreeAsset visualTree = Resources.Load("ProgressionFormulaUXML") as VisualTreeAsset;
            if (visualTree != null)
                progressionFormulaElement = visualTree.Instantiate(property.propertyPath);

            StyleSheet ss = Resources.Load("ProgressionFormulaUSS") as StyleSheet;
            progressionFormulaElement.styleSheets.Add(ss);

            VisualElement levelsScrollView = progressionFormulaElement.Q("LevelsScrollView");
            if (levelsScrollView == null) return progressionFormulaElement;

            VisualElement levelValuesContainer = levelsScrollView.Q("LevelValues");
            if (levelValuesContainer == null) return progressionFormulaElement;

            SerializedProperty startingValue = property.FindPropertyRelative("startingValue");
            SerializedProperty percentageAdd = property.FindPropertyRelative("percentageAdd");
            SerializedProperty absoluteAdd = property.FindPropertyRelative("absoluteAdd");
            SerializedProperty useCurve = property.FindPropertyRelative("useCurve");
            SerializedProperty curve = property.FindPropertyRelative("curve");
            FloatField[] levelValues = new FloatField[100];

            for (var i = 0; i < 100; i++)
            {
                levelValues[i] = levelValuesContainer.Q<FloatField>($"LevelValue{i + 1}");
            }

            CalculateLevels(levelValues, startingValue.floatValue, percentageAdd.floatValue, absoluteAdd.floatValue,
                useCurve.boolValue, curve.animationCurveValue);

            progressionFormulaElement.Q<Slider>("startingValueSlider").RegisterValueChangedCallback(evt =>
            {
                CalculateLevels(levelValues, evt.newValue, percentageAdd.floatValue, absoluteAdd.floatValue,
                    useCurve.boolValue, curve.animationCurveValue);
            });
            progressionFormulaElement.Q<Slider>("percentageAddSlider")?.RegisterValueChangedCallback(evt =>
            {
                CalculateLevels(levelValues, startingValue.floatValue, evt.newValue, absoluteAdd.floatValue,
                    useCurve.boolValue, curve.animationCurveValue);
            });
            progressionFormulaElement.Q<Slider>("absoluteAddSlider")?.RegisterValueChangedCallback(evt =>
            {
                CalculateLevels(levelValues, startingValue.floatValue, percentageAdd.floatValue, evt.newValue,
                    useCurve.boolValue, curve.animationCurveValue);
            });
            progressionFormulaElement.Q<Toggle>("useCurve")?.RegisterValueChangedCallback(evt =>
            {
                CalculateLevels(levelValues, startingValue.floatValue, percentageAdd.floatValue, absoluteAdd.floatValue,
                    evt.newValue, curve.animationCurveValue);
            });
            progressionFormulaElement.Q<CurveField>("curve")?.RegisterValueChangedCallback(evt =>
            {
                CalculateLevels(levelValues, startingValue.floatValue, percentageAdd.floatValue, absoluteAdd.floatValue,
                    useCurve.boolValue, evt.newValue);
            });

            return progressionFormulaElement;
        }

        private void CalculateLevels(FloatField[] levelValues, float startingValue, float percentageAdd, float absoluteAdd,
            bool useCurve, AnimationCurve curve)
        {
            for (var i = 0; i < 100; i++)
            {
                if (levelValues[i] != null)
                    levelValues[i].SetValueWithoutNotify(Calculate(i + 1, startingValue, percentageAdd, absoluteAdd, useCurve, curve));
            }
        }

        private float Calculate(int level, float startingValue, float percentageAdd, float absoluteAdd, bool useCurve,
            AnimationCurve curve)
        {
            if (level <= 1) return startingValue;
            var c = Calculate(level - 1, startingValue, percentageAdd, absoluteAdd, useCurve, curve);
            var value = c + c * percentageAdd + absoluteAdd;
            if (!useCurve) return value;
            return value + c * curve.Evaluate(1f / level);
        }
    }
}