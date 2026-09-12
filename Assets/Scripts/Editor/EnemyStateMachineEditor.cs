#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace AppleGrapple
{
    [CustomEditor(typeof(EnemyStateMachine))]
    public class EnemyStateMachineEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("AI Monitor", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(true))
            {
                DrawMonitorField("Current State", "_debugCurrentState");
                DrawMonitorField("Last Decision", "_debugLastDecision");
                DrawMonitorField("Target", "_debugTarget");
                DrawMonitorField("Own Swords", "_debugSwordCount");
                DrawMonitorField("Target Swords", "_debugTargetSwordCount");
                DrawMonitorField("Health %", "_debugHealthPercent");
                DrawMonitorField("Movement Input", "_debugMovementInput");
            }

            if (Application.isPlaying)
                Repaint();
        }

        private void DrawMonitorField(string label, string propertyName)
        {
            var property = serializedObject.FindProperty(propertyName);
            if (property != null)
                EditorGUILayout.PropertyField(property, new GUIContent(label));
        }
    }
}
#endif
