// ReadOnlyAttributePropertyDrawer.cs
// 07-13-2022
// James LaFritz

using RPGEngine.Core;
using UnityEditor;
using UnityEngine;

namespace RPGEditor.Core
{
    /// <summary>
    /// A property drawer for ReadOnlyAttribute
    /// <seealso href="https://docs.unity3d.com/ScriptReference/PropertyDrawer.html"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributePropertyDrawer : PropertyDrawer
    {
        #region Property Drawer Overrides

        public override void OnGUI(Rect position, SerializedProperty property,
                                   GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }

        #endregion
    }
}