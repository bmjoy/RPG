using UnityEditor;
using UnityEngine;
using static RPG.Core.StringReferences;

namespace RPG.Core.Editor
{
    public class StringReferencesEditor : EditorWindow
    {
        [MenuItem("Window/RPG Tool Kit/String References Editor")]
        public static void ShowExample()
        {
            StringReferencesEditor wnd = GetWindow<StringReferencesEditor>();
            wnd.titleContent = new GUIContent("String References");
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                GUILayout.Label("Animator Parameters", EditorStyles.boldLabel);

                forwardSpeedFloat = EditorGUILayout.TextField("Forward Speed Float", forwardSpeedFloat);
                attackTrigger = EditorGUILayout.TextField("Attack Trigger", attackTrigger);
                stopAttackTrigger = EditorGUILayout.TextField("Stop Attack Trigger", stopAttackTrigger);
                deathTrigger = EditorGUILayout.TextField("Death Trigger", deathTrigger);
            }
        }
    }
}