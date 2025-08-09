#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(CustomButton), true)]
[CanEditMultipleObjects]
public class CustomButtonEditor : ButtonEditor
{
    SerializedProperty playSoundTypeProp;

    protected override void OnEnable()
    {
        base.OnEnable();
        playSoundTypeProp = serializedObject.FindProperty("playSoundType");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();              // Button 기본 UI
        serializedObject.Update();
        EditorGUILayout.PropertyField(playSoundTypeProp);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif