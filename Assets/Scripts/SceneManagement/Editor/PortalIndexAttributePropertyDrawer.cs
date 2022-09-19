// PortalIndexAttributePropertyDrawer.cs
// 07-12-2022
// James LaFritz

using System.Collections.Generic;
using System.Linq;
using RPGEditor.Core;
using RPGEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.SceneManagement.EditorSceneManager;

namespace RPGEditor.SceneManagement
{
    /// <summary>
    /// The portal index attribute property drawer class
    /// <seealso href="https://docs.unity3d.com/ScriptReference/PropertyDrawer.html"/>
    /// </summary>
    [CustomPropertyDrawer(typeof(PortalIndexAttribute))]
    public class PortalIndexAttributePropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// The errormessage
        /// </summary>
        private string _errorMessage;

        #region Overrides of PropertyDrawer

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Application.isPlaying)
            {
                // ReSharper disable once Unity.PropertyDrawerOnGUIBase
                base.OnGUI(position, property, label);
                return;
            }

            if (property == null) return;

            if (property.propertyType != SerializedPropertyType.Integer)
            {
                _errorMessage = $"{nameof(PortalIndexAttribute)} can only be used on integer properties.";
                Debug.LogError(_errorMessage);
                EditorGUI.LabelField(position, label.text, _errorMessage);
                return;
            }

            PortalIndexAttribute attr = attribute as PortalIndexAttribute;

            SerializedProperty sceneProp =
                PropertyDrawerHelper.FindProperty(property, attr.scene, out _errorMessage);
            if (sceneProp == null)
            {
                Debug.LogError(_errorMessage);
                EditorGUI.LabelField(position, label.text, _errorMessage);
                return;
            }

            if (sceneProp.propertyType != SerializedPropertyType.Integer)
            {
                _errorMessage = $"{nameof(sceneProp)} must be an int field.";
                Debug.LogError(_errorMessage);
                EditorGUI.LabelField(position, label.text, _errorMessage);
                return;
            }

            string[] portalNames = PortalNames(sceneProp);

            using (new EditorGUI.PropertyScope(position, label, property))
            {
                using (EditorGUI.ChangeCheckScope changeCheck = new EditorGUI.ChangeCheckScope())
                {
                    DrawPortalPopup(position, property, label, portalNames);

                    if (changeCheck.changed)
                    {
                        property.serializedObject?.ApplyModifiedProperties();
                    }
                }
            }
        }

        #endregion

        private static string[] PortalNames(SerializedProperty sceneProp)
        {
            EditorBuildSettingsScene editorScene = EditorBuildSettings.scenes[sceneProp.intValue];
            Scene scene = SceneManager.GetActiveScene().buildIndex != sceneProp.intValue
                ? OpenScene(editorScene.path, OpenSceneMode.Additive)
                : SceneManager.GetActiveScene();

            GameObject[] sceneObjects = scene.GetRootGameObjects();
            List<Portal> portalObjects = sceneObjects!.SelectMany(sceneObject => sceneObject!.GetComponentsInChildren<Portal>()).ToList();

            int portalIndex = 0;
            foreach (Portal portal in portalObjects.OrderBy(a => a!.Index))
            {
                if (portal!.Index != portalIndex) portal.Index = portalIndex;
                portalIndex++;
            }

            string[] portalNames = portalObjects.Select(a => a.name).ToArray();
            if (scene != SceneManager.GetActiveScene()) CloseScene(scene, true);
            return portalNames;
        }

        /// <summary>
        /// Draws a popup selection for the portal index by the name of the portal.
        /// </summary>
        /// <param name="rect">The rect</param>
        /// <param name="property">The property</param>
        /// <param name="label">The label</param>
        /// <param name="portalOptions">The portal options</param>
        private static void DrawPortalPopup(Rect rect, SerializedProperty property, GUIContent label,
                                            string[] portalOptions)
        {
            if (property == null) return;

            int index = property.intValue;
            int newIndex = EditorGUI.Popup(rect, label != null ? label.text : "", index, portalOptions);

            if (property.intValue != newIndex)
            {
                property.intValue = newIndex;
            }
        }
    }
}