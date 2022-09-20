using UnityEditor;
using UnityEngine;
using static RPGEngine.Core.StringReferences;

namespace RPGEditor.Core.Editor
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

                ForwardSpeedFloat = EditorGUILayout.TextField("Forward Speed Float", ForwardSpeedFloat);
                AttackTrigger = EditorGUILayout.TextField("Attack Trigger", AttackTrigger);
                StopAttackTrigger = EditorGUILayout.TextField("Stop Attack Trigger", StopAttackTrigger);
                DeathTrigger = EditorGUILayout.TextField("Death Trigger", DeathTrigger);
            }
        }
    }
}