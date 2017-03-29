namespace CustomEditor
{
    using Combat.Board;

    using UnityEditor;
    using UnityEditor.UI;

    using UnityEngine;

    [CustomEditor(typeof(GemImage))]
    public class GemImageEditor : ImageEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();

            var iter = serializedObject.FindProperty("m_FillOrigin");
            while (iter.NextVisible(false))
                EditorGUILayout.PropertyField(iter,true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
