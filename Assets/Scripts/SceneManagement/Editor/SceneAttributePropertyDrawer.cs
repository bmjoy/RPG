// SceneAttributePropertyDrawer.cs
// 07-12-2022
// James LaFritz

using System.Linq;
using System.Text.RegularExpressions;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace RPG.SceneManagement.Editor
{
    /// <summary>
    /// The scene attribute property drawer class
    /// <seealso href="https://docs.unity3d.com/ScriptReference/PropertyDrawer.html"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class SceneAttributePropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// Gets the value of the any scene in build settings
        /// </summary>
        private static bool AnySceneInBuildSettings => GetScenes()?.Length > 0;

        /// <summary>
        /// Gets the scenes
        /// </summary>
        /// <returns>The string array</returns>
        private static string[] GetScenes()
        {
            return (from scene in EditorBuildSettings.scenes
                    where scene.enabled
                    select scene.path).ToArray();
        }

        /// <summary>
        /// Gets the scene options using the specified scenes.
        /// Uses Regex to remove the path from the scene name.
        /// </summary>
        /// <param name="scenes">The scenes</param>
        /// <returns>The string array</returns>
        private static string[] GetSceneOptions(string[] scenes)
        {
            return (from scene in scenes
                    select Regex.Match(scene ?? string.Empty,
                                       @".+\/(.+).unity").Groups[1].Value).ToArray();
        }

        #region Overrides of PropertyDrawer

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property == null) return;

            if (!(property.propertyType == SerializedPropertyType.String ||
                  property.propertyType == SerializedPropertyType.Integer))
            {
                Debug.LogError($"{nameof(SceneAttribute)} supports only string and int fields");
                label.text = $"{nameof(SceneAttribute)} supports only string fields and int fields";
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            if (!AnySceneInBuildSettings)
            {
                EditorGUI.HelpBox(
                    position,
                    "No scenes in the build settings, Please ensure that you add your scenes to File->Build Settings->",
                    MessageType.Error);
                return;
            }

            string[] scenes = GetScenes();
            string[] sceneOptions = GetSceneOptions(scenes);

            using (new EditorGUI.PropertyScope(position, label, property))
            {
                using (EditorGUI.ChangeCheckScope changeCheck = new EditorGUI.ChangeCheckScope())
                {
                    switch (property.propertyType)
                    {
                        case SerializedPropertyType.String:
                            DrawPropertyForString(position, property, label, scenes, sceneOptions);
                            break;
                        case SerializedPropertyType.Integer:
                            DrawPropertyForInt(position, property, label, sceneOptions);
                            break;
                        default:
                            Debug.LogError(
                                $"{nameof(SceneAttribute)} supports only int or string fields! {property.propertyType}");
                            break;
                    }

                    if (changeCheck.changed)
                    {
                        property.serializedObject?.ApplyModifiedProperties();
                    }
                }
            }
        }

        #endregion

        #region Drawer Methods

        /// <summary>
        /// Draws the property for string using the specified rect
        /// </summary>
        /// <param name="rect">The rect</param>
        /// <param name="property">The property</param>
        /// <param name="label">The label</param>
        /// <param name="scenes">The scenes</param>
        /// <param name="sceneOptions">The scene options</param>
        private static void DrawPropertyForString(Rect rect, SerializedProperty property, GUIContent label,
                                                  string[] scenes, string[] sceneOptions)
        {
            if (property == null) return;
            if (scenes == null) return;

            int index = math.clamp(System.Array.IndexOf(scenes, property.stringValue), 0, scenes.Length - 1);
            int newIndex = EditorGUI.Popup(rect, label != null ? label.text : "", index, sceneOptions);
            string newScene = scenes[newIndex];

            if (property.stringValue?.Equals(newScene, System.StringComparison.Ordinal) == false)
            {
                property.stringValue = scenes[newIndex];
            }
        }

        /// <summary>
        /// Draws the property for int using the specified rect
        /// </summary>
        /// <param name="rect">The rect</param>
        /// <param name="property">The property</param>
        /// <param name="label">The label</param>
        /// <param name="sceneOptions">The scene options</param>
        private static void DrawPropertyForInt(Rect rect, SerializedProperty property, GUIContent label,
                                               string[] sceneOptions)
        {
            if (property == null) return;

            int index = property.intValue;
            int newIndex = EditorGUI.Popup(rect, label != null ? label.text : "", index, sceneOptions);

            if (property.intValue != newIndex)
            {
                property.intValue = newIndex;
            }
        }

        #endregion
    }
}