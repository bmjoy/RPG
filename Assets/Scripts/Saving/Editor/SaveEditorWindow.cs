// SaveEditorWindow.cs
// 07-13-2022
// James LaFritz

using System.IO;
using RPGEngine.Saving;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Screen;

namespace RPGEditor.Saving
{
    /// <summary>
    /// A custom editor window
    /// <seealso href="https://docs.unity3d.com/ScriptReference/EditorWindow.html"/>
    /// </summary>
    public class SaveEditorWindow : EditorWindow
    {
        private const float WindowWidth = 400;
        private const float WindowHeight = 200;

        private const string HelpBoxText =
            "Every Save File has a Version Number. When trying to load a save, only files with the current version (or the minimum legacy version) will be valid.";

        private int m_cachedVersionNumber;
        private int m_cachedMinVersionNumber;
        private bool m_legacySupport;
        private GUIStyle m_centeredLabel;

        #region Window Managment

        [MenuItem("Window/RPG Tool Kit/Save Settings")]
        private static void ShowWindow()
        {
            SaveEditorWindow window = GetWindow<SaveEditorWindow>("Save Settings", true);

            //Set default size & position
            Rect windowRect = new Rect()
            {
                size = new Vector2(WindowWidth, WindowHeight),
                x = (float)currentResolution.width / 2 - WindowWidth,
                y = (float)currentResolution.height / 2 - WindowHeight
            };
            window.position = windowRect;

            window.m_legacySupport = VersionControl.minFileVersion != VersionControl.currentFileVersion;
            window.m_cachedVersionNumber = VersionControl.currentFileVersion;
            window.m_cachedMinVersionNumber = VersionControl.minFileVersion;

            window.m_centeredLabel = EditorStyles.boldLabel;
            window.m_centeredLabel.alignment = TextAnchor.MiddleCenter;
            window.Show();
        }

        /// <summary>
        /// <seealso href="https://docs.unity3d.com/ScriptReference/EditorWindow.OnGUI.html"/>
        /// </summary>
        private void OnGUI()
        {
            // Help Box
            EditorGUILayout.HelpBox(HelpBoxText, MessageType.Info);
            EditorGUILayout.Space();

            //Version Number Editing
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Current Version NR.", GUILayout.MaxWidth(150));
            if (m_cachedVersionNumber == 1) GUI.enabled = false;
            if (GUILayout.Button("-", GUILayout.MaxWidth(25))) ShiftCurrentVersion(-1);
            if (!GUI.enabled) GUI.enabled = true;
            EditorGUILayout.LabelField($"{m_cachedVersionNumber}", m_centeredLabel, GUILayout.MaxWidth(35));
            if (GUILayout.Button("+", GUILayout.MaxWidth(25))) ShiftCurrentVersion(1);
            EditorGUILayout.EndHorizontal();

            // Legacy Version Number Editing
            m_legacySupport = EditorGUILayout.Toggle("Backwards Compatibility", m_legacySupport);
            if (m_legacySupport)
            {
                if (m_cachedMinVersionNumber > m_cachedVersionNumber)
                {
                    m_cachedMinVersionNumber = m_cachedVersionNumber;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Min. Version NR.", GUILayout.MaxWidth(150));
                if (m_cachedMinVersionNumber <= 1) GUI.enabled = false;
                if (GUILayout.Button("-", GUILayout.MaxWidth(25))) ShiftMinVersion(-1);
                if (!GUI.enabled) GUI.enabled = true;
                EditorGUILayout.LabelField($"{m_cachedMinVersionNumber}", m_centeredLabel, GUILayout.MaxWidth(35));
                if (m_cachedMinVersionNumber >= m_cachedVersionNumber) GUI.enabled = false;
                if (GUILayout.Button("+", GUILayout.MaxWidth(25))) ShiftMinVersion(1);
                if (!GUI.enabled) GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                m_cachedMinVersionNumber = m_cachedVersionNumber;
            }

            // Apply Changes
            if (m_cachedVersionNumber == VersionControl.currentFileVersion &&
                m_cachedMinVersionNumber == VersionControl.minFileVersion)
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("Apply")) UpdateVersionNumber();
            if (!GUI.enabled) GUI.enabled = true;

            // Source Folder Access
            if (GUILayout.Button("Open Source Folder"))
                System.Diagnostics.Process.Start(GetSaveFolderPath()!);
        }

        #endregion

        #region Version Number Management

        private void ShiftCurrentVersion(int increment)
        {
            m_cachedVersionNumber += increment;

            if (!m_legacySupport)
            {
                m_cachedMinVersionNumber = m_cachedVersionNumber;
            }
        }

        private void ShiftMinVersion(int increment)
        {
            m_cachedMinVersionNumber += increment;
        }

        private void UpdateVersionNumber()
        {
            VersionControl.currentFileVersion = m_cachedVersionNumber;
            VersionControl.minFileVersion = m_cachedMinVersionNumber;
        }

        #endregion

        #region Path Management

        private string GetSaveFolderPath()
        {
            string basePath = Path.Combine(Application.persistentDataPath!, "GameData");

            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

            return basePath;
        }

        #endregion
    }
}