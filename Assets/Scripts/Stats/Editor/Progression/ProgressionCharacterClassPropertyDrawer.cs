using RPGEngine.Stats;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGEditor.Stats
{
    //[CustomPropertyDrawer(typeof(Progression.ProgressionCharacterClass))]
    public class ProgressionCharacterClassPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty characterClassProperty = property.FindPropertyRelative("characterClass");
            SerializedProperty statsProperty = property.FindPropertyRelative("stats.Array");

            string className = ((CharacterClass)characterClassProperty.enumValueIndex).ToString();

            VisualElement root = new();
            VisualTreeAsset visualTree = Resources.Load("ProgressionCharacterClassUXML") as VisualTreeAsset;
            if (visualTree != null)
                root = visualTree.Instantiate(property.propertyPath).contentContainer;

            return root;
        }
    }
}