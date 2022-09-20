// ShowIfBoolPropertyDrawer.cs
// 07-12-2022
// James LaFritz

using RPGEngine.Core;
using UnityEditor;
using UnityEngine;

namespace RPGEditor.Core.Editor
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
        private string _errorMessage;

        #region Overrides of PropertyDrawer

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property,
                                   GUIContent label)
        {
            if (attribute is not ShowIfBoolAttribute attr) return;
            SerializedProperty showIfProp = PropertyDrawerHelper.FindProperty(property, attr.BoolName, out _errorMessage);
            if (showIfProp == null)
            {
                EditorGUI.LabelField(position, label.text, _errorMessage);
                return;
            }

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                if ((showIfProp.boolValue && attr.Show) ||
                    (!showIfProp.boolValue && !attr.Show))
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }

            EditorGUI.indentLevel = indent;
        }

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (attribute is not ShowIfBoolAttribute attr) return base.GetPropertyHeight(property, label);
            SerializedProperty showIfProp = PropertyDrawerHelper.FindProperty(property, attr.BoolName, out _errorMessage);
            if (showIfProp == null) return base.GetPropertyHeight(property, label);
            if ((showIfProp.boolValue && attr.Show) ||
                (!showIfProp.boolValue && !attr.Show))
                return EditorGUI.GetPropertyHeight(property, label, true);
            return -EditorGUIUtility.standardVerticalSpacing;
        }

        #endregion
    }
}