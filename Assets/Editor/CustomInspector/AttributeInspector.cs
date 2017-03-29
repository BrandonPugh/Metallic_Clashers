namespace CustomInspector
{
    using UnityEditor;

    using UnityEngine;
    using UnityEngine.Events;

    [CustomPropertyDrawer(typeof(Attribute))]
    public class AttributeInspector : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                --EditorGUI.indentLevel;

                position.width /= 5f;

                var valueProperty = property.FindPropertyRelative("value");
                EditorGUI.PropertyField(
                    position, valueProperty, GUIContent.none);

                var plusRect = position;
                plusRect.position += new Vector2(position.width, 0f);
                plusRect.width /= 3.5f;

                var centeredStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.LowerCenter, };
                EditorGUI.LabelField(plusRect, "+", centeredStyle);

                position.x = plusRect.x + plusRect.width;

                var modifier = property.FindPropertyRelative("m_Modifier");
                EditorGUI.PropertyField(
                    position,
                    property.FindPropertyRelative("m_Modifier"),
                    GUIContent.none);

                var multRect = plusRect;
                multRect.position = position.position + new Vector2(position.width, 0f);

                EditorGUI.LabelField(multRect, "*", centeredStyle);

                position.x = multRect.x + multRect.width;

                var coefficient = property.FindPropertyRelative("m_Coefficient");
                EditorGUI.PropertyField(
                    position,
                    property.FindPropertyRelative("m_Coefficient"),
                    GUIContent.none);

                position.position += new Vector2(position.width, 0f);

                EditorGUI.LabelField(
                    position, " = "
                    + (valueProperty.floatValue + modifier.floatValue) * coefficient.floatValue);

                ++EditorGUI.indentLevel;
            }
            EditorGUI.EndProperty();
        }
    }
}
