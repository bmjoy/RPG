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

        private int _cachedVersionNumber;
        private int _cachedMinVersionNumber;
        private bool _legacySupport;
        private GUIStyle _centeredLabel;

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

            window._legacySupport = VersionControl.minFileVersion != VersionControl.currentFileVersion;
            window._cachedVersionNumber = VersionControl.currentFileVersion;
            window._cachedMinVersionNumber = VersionControl.minFileVersion;

            window._centeredLabel = EditorStyles.boldLabel;
            window._centeredLabel.alignment = TextAnchor.MiddleCenter;
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
            if (_cachedVersionNumber == 1) GUI.enabled = false;
            if (GUILayout.Button("-", GUILayout.MaxWidth(25))) ShiftCurrentVersion(-1);
            if (!GUI.enabled) GUI.enabled = true;
            EditorGUILayout.LabelField($"{_cachedVersionNumber}", _centeredLabel, GUILayout.MaxWidth(35));
            if (GUILayout.Button("+", GUILayout.MaxWidth(25))) ShiftCurrentVersion(1);
            EditorGUILayout.EndHorizontal();

            // Legacy Version Number Editing
            _legacySupport = EditorGUILayout.Toggle("Backwards Compatibility", _legacySupport);
            if (_legacySupport)
            {
                if (_cachedMinVersionNumber > _cachedVersionNumber)
                {
                    _cachedMinVersionNumber = _cachedVersionNumber;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Min. Version NR.", GUILayout.MaxWidth(150));
                if (_cachedMinVersionNumber <= 1) GUI.enabled = false;
                if (GUILayout.Button("-", GUILayout.MaxWidth(25))) ShiftMinVersion(-1);
                if (!GUI.enabled) GUI.enabled = true;
                EditorGUILayout.LabelField($"{_cachedMinVersionNumber}", _centeredLabel, GUILayout.MaxWidth(35));
                if (_cachedMinVersionNumber >= _cachedVersionNumber) GUI.enabled = false;
                if (GUILayout.Button("+", GUILayout.MaxWidth(25))) ShiftMinVersion(1);
                if (!GUI.enabled) GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                _cachedMinVersionNumber = _cachedVersionNumber;
            }

            // Apply Changes
            if (_cachedVersionNumber == VersionControl.currentFileVersion &&
                _cachedMinVersionNumber == VersionControl.minFileVersion)
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
            _cachedVersionNumber += increment;

            if (!_legacySupport)
            {
                _cachedMinVersionNumber = _cachedVersionNumber;
            }
        }

        private void ShiftMinVersion(int increment)
        {
            _cachedMinVersionNumber += increment;
        }

        private void UpdateVersionNumber()
        {
            VersionControl.currentFileVersion = _cachedVersionNumber;
            VersionControl.minFileVersion = _cachedMinVersionNumber;
        }

        #endregion

        #region Path Management

        private string GetSaveFolderPath()
        {
            string basePath = SavingStrategy.BasePath;

            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath!);

            return basePath;
        }

        #endregion
    }
}