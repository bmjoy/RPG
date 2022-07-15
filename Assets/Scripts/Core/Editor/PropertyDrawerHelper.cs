// PropertyDrawerHelper.cs
// 07-12-2022
// James LaFritz

using UnityEditor;

namespace RPGEditor.Core
{
    /// <summary>
    /// Collection of helper methods when coding a PropertyDrawer editor.
    /// </summary>
    public static class PropertyDrawerHelper
    {
        /// <summary>
        /// Finds the property using the specified property
        /// </summary>
        /// <param name="property">The property to look in for the property name.</param>
        /// <param name="propertyName">The property name</param>
        /// <param name="errorMessage">The error message</param>
        /// <returns>The serialized property</returns>
        public static SerializedProperty FindProperty(SerializedProperty property, string propertyName,
                                                      out string errorMessage)
        {
            SerializedProperty prop = property.serializedObject.FindProperty(propertyName);
            errorMessage = string.Empty;
            if (prop != null) return prop;
            string propPath =
                property.propertyPath.Substring(
                    0, property.propertyPath.IndexOf($".{property.name}", System.StringComparison.Ordinal));
            prop = property.serializedObject.FindProperty($"{propPath}.{propertyName}");
            if (prop != null) return prop;
            errorMessage = $"The Field name {propertyName} cannot be found in {propPath}";
            return null;
        }
    }
}