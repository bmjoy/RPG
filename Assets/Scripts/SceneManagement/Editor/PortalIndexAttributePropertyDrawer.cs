// PortalIndexAttributePropertyDrawer.cs
// 07-12-2022
// James LaFritz

using System.Collections.Generic;
using System.Linq;
using RPG.Core.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement.Editor
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
        private string m_errorMessage;

        #region Overrides of PropertyDrawer

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property == null) return;

            if (property.propertyType != SerializedPropertyType.Integer)
            {
                m_errorMessage = $"{nameof(PortalIndexAttribute)} can only be used on integer properties.";
                Debug.LogError(m_errorMessage);
                EditorGUI.LabelField(position, label.text, m_errorMessage);
                return;
            }

            PortalIndexAttribute attr = attribute as PortalIndexAttribute;

            SerializedProperty sceneProp =
                PropertyDrawerHelper.FindProperty(property, attr.scene, out m_errorMessage);
            if (sceneProp == null)
            {
                Debug.LogError(m_errorMessage);
                EditorGUI.LabelField(position, label.text, m_errorMessage);
                return;
            }

            if (sceneProp.propertyType != SerializedPropertyType.Integer)
            {
                m_errorMessage = $"{nameof(sceneProp)} must be an int field.";
                Debug.LogError(m_errorMessage);
                EditorGUI.LabelField(position, label.text, m_errorMessage);
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
            Scene scene = EditorSceneManager.GetActiveScene().buildIndex != sceneProp.intValue
                ? EditorSceneManager.OpenScene(editorScene.path, OpenSceneMode.Additive)
                : EditorSceneManager.GetActiveScene();

            GameObject[] sceneObjects = scene.GetRootGameObjects();
            List<Portal> portalObjects = new List<Portal>();
            foreach (GameObject sceneObject in sceneObjects)
            {
                Portal[] portals = sceneObject.GetComponentsInChildren<Portal>();
                foreach (Portal portal in portals)
                {
                    portalObjects.Add(portal);
                }
            }

            int portalIndex = 0;
            foreach (Portal portal in portalObjects.OrderBy(a => a.Index))
            {
                if (portal.Index != portalIndex) portal.Index = portalIndex;
                portalIndex++;
            }

            string[] portalNames = portalObjects.Select(a => a.name).ToArray();
            if (scene != EditorSceneManager.GetActiveScene()) EditorSceneManager.CloseScene(scene, true);
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