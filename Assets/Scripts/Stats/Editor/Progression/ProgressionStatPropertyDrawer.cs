using RPGEngine.Stats;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGEditor.Stats
{
    //[CustomPropertyDrawer(typeof(Progression.ProgressionStat))]
    public class ProgressionStatPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty statProperty = property.FindPropertyRelative("stat");
            string statName = ((Stat)statProperty.enumValueIndex).ToString();

            VisualElement root = new();
            VisualTreeAsset visualTree = Resources.Load("ProgressionStatUXML") as VisualTreeAsset;
            if (visualTree != null)
                root = visualTree.Instantiate(property.propertyPath);

            switch (statProperty.enumValueIndex)
            {
                case (int)Stat.ExperienceToLevel:
                    root.Add(new Label("Cumulative Experience"));
                    root.Add(new Label("Experience per enemy kill"));
                    root.Add(new Label("Enemies to kill to level up"));
                    break;
                case (int)Stat.Health:
                    root.Add(new Label("Total Hit To Die"));
                    break;
                case (int)Stat.Damage:
                {
                    root.Add(new Label("Total Hit Damage"));
                    break;
                }
            }

            return root;
        }
    }
}