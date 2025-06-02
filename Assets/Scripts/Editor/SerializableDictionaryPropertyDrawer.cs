#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
public class SerializableDictionaryPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded, label, true);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            SerializedProperty keysProp = property.FindPropertyRelative("keys");
            SerializedProperty valuesProp = property.FindPropertyRelative("values");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = 4f;
            float y = position.y + lineHeight + spacing;
            float fieldHeight = lineHeight * 1.2f;
            float fieldWidth = (position.width - 50) / 2f; 

            int removeIndex = -1;

            for (int i = 0; i < keysProp.arraySize; i++)
            {
                Rect keyRect = new Rect(position.x, y, fieldWidth, fieldHeight);
                Rect valueRect = new Rect(position.x + fieldWidth + 10, y, fieldWidth, fieldHeight);
                Rect removeButtonRect = new Rect(position.x + fieldWidth * 2 + 20, y, 30, fieldHeight);

                EditorGUI.PropertyField(keyRect, keysProp.GetArrayElementAtIndex(i), GUIContent.none);
                EditorGUI.PropertyField(valueRect, valuesProp.GetArrayElementAtIndex(i), GUIContent.none);

                Color originalColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUI.Button(removeButtonRect, "X"))
                {
                    removeIndex = i;
                }
                GUI.backgroundColor = originalColor;

                y += fieldHeight + spacing;
            }

            if (removeIndex >= 0)
            {
                keysProp.DeleteArrayElementAtIndex(removeIndex);
                valuesProp.DeleteArrayElementAtIndex(removeIndex);
            }

            // Add Entry 버튼
            float addButtonHeight = fieldHeight * 1.2f;
            Rect addButtonRect = new Rect(position.x, y + spacing, position.width, addButtonHeight);
            if (GUI.Button(addButtonRect, "+ Add Entry"))
            {
                keysProp.arraySize++;
                valuesProp.arraySize++;

                SerializedProperty newKey = keysProp.GetArrayElementAtIndex(keysProp.arraySize - 1);
                SerializedProperty newValue = valuesProp.GetArrayElementAtIndex(valuesProp.arraySize - 1);

                System.Type dictionaryType = fieldInfo.FieldType;
                System.Type keyType = dictionaryType.GetGenericArguments()[0];
                System.Type valueType = dictionaryType.GetGenericArguments()[1];

                if (keyType.IsEnum)
                {
                    newKey.enumValueIndex = 0;
                }
                else if (keyType == typeof(string))
                {
                    newKey.stringValue = "";
                }

                if (valueType.IsEnum)
                {
                    newValue.enumValueIndex = 0;
                }
                else if (valueType == typeof(string))
                {
                    newValue.stringValue = "";
                }
            }

            EditorGUI.indentLevel--;
        }

        
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (property.isExpanded)
        {
            SerializedProperty keysProp = property.FindPropertyRelative("keys");
            int arraySize = keysProp.arraySize;

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = 4f;
            float fieldHeight = lineHeight * 1.2f;
            float addButtonHeight = fieldHeight * 1.2f;

            height += (fieldHeight + spacing) * arraySize;
            height += addButtonHeight + spacing * 2;
        }

        return height;
    }
}
#endif
