// ShowIfBoolPropertyDrawer.cs
// 07-12-2022
// James LaFritz

using UnityEditor;
using UnityEngine;

namespace RPG.Core.Editor
{
    /// <summary>
    /// The show if bool property drawer class
    /// </summary>
    /// <seealso cref="PropertyDrawer"/>
    [CustomPropertyDrawer(typeof(ShowIfBoolAttribute))]
    public class ShowIfBoolPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// The errormessage
        /// </summary>
        private string m_errorMessage;

        #region Overrides of PropertyDrawer

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property,
                                   GUIContent label)
        {
            ShowIfBoolAttribute attr = attribute as ShowIfBoolAttribute;
            SerializedProperty showIfProp = PropertyDrawerHelper.FindProperty(property, attr.boolName, out m_errorMessage);
            if (showIfProp == null)
            {
                EditorGUI.LabelField(position, label.text, m_errorMessage);
                return;
            }

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                if (showIfProp.boolValue && attr.show ||
                    !showIfProp.boolValue && !attr.show)
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }

            EditorGUI.indentLevel = indent;
        }

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowIfBoolAttribute attr = attribute as ShowIfBoolAttribute;
            SerializedProperty showIfProp = PropertyDrawerHelper.FindProperty(property, attr.boolName, out m_errorMessage);
            if (showIfProp == null) return base.GetPropertyHeight(property, label);
            if (showIfProp.boolValue && attr.show ||
                !showIfProp.boolValue && !attr.show)
                return EditorGUI.GetPropertyHeight(property, label, true);
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        #endregion
    }
}