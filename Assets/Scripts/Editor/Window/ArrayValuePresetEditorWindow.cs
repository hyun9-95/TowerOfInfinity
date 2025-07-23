using UnityEditor;
using UnityEngine;
using System;

public class ArrayValuePresetEditorWindow : EditorWindow
{
    private SerializedProperty targetProperty;
    private int arraySize;
    private EditorGraph selectedGraph = EditorGraph.Linear;

    // Linear
    private float linearStartValue = 0f;
    private float linearIncrement = 1f;

    // Step
    private float stepStartValue = 0f;
    private float stepIncrement = 1f;
    private int stepInterval = 1;

    // Log
    private float logStartValue = 1f; // 0이 될 수 없음
    private float logFactor = 1.1f;
    private float initialValue;

    public static void ShowWindow(SerializedProperty property, int size, float initialValue_)
    {
        ArrayValuePresetEditorWindow window = GetWindow<ArrayValuePresetEditorWindow>("Array Value Preset");
        window.targetProperty = property;
        window.arraySize = size;
        window.initialValue = initialValue_;
        window.InitializeWindow();
        window.InitializeWindow(300, 150);
    }

    private void InitializeWindow()
    {
        linearStartValue = initialValue;
        stepStartValue = initialValue;
        logStartValue = initialValue; // Log는 0이 될 수 없으므로 주의
    }

    private void OnGUI()
    {
        if (targetProperty == null || !targetProperty.isArray)
        {
            EditorGUILayout.HelpBox("유효한 배열 Property가 선택되지 않았습니다.", MessageType.Error);
            return;
        }

        EditorGUILayout.LabelField("Array Value Preset", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        selectedGraph = (EditorGraph)EditorGUILayout.EnumPopup("그래프 타입", selectedGraph);
        
        EditorGUILayout.Space();

        DrawGraphSettings();

        EditorGUILayout.Space();
        if (GUILayout.Button("Apply", GUILayout.Height(30)))
        {
            ApplyPreset();
        }
    }

    private void DrawGraphSettings()
    {
        switch (selectedGraph)
        {
            case EditorGraph.Linear:
                linearStartValue = EditorGUILayout.FloatField("시작 값", linearStartValue);
                linearIncrement = EditorGUILayout.FloatField("증가량", linearIncrement);
                break;
            case EditorGraph.Step:
                stepStartValue = EditorGUILayout.FloatField("시작 값", stepStartValue);
                stepIncrement = EditorGUILayout.FloatField("증가량", stepIncrement);
                stepInterval = EditorGUILayout.IntField("단계 간격", stepInterval);
                stepInterval = Mathf.Max(1, stepInterval); // 최소 1
                break;
            case EditorGraph.Log:
                logStartValue = EditorGUILayout.FloatField("시작 값 (0 이상)", logStartValue);
                logStartValue = Mathf.Max(0.001f, logStartValue); // 0 방지
                logFactor = EditorGUILayout.FloatField("계수", logFactor);
                logFactor = Mathf.Max(1.001f, logFactor); // 1 이상
                break;
        }
    }

    private void ApplyPreset()
    {
        targetProperty.serializedObject.Update();
        targetProperty.arraySize = arraySize;

        for (int i = 0; i < arraySize; i++)
        {
            float value = CalculateValue(i);
            SerializedProperty element = targetProperty.GetArrayElementAtIndex(i);

            switch (element.propertyType)
            {
                case SerializedPropertyType.Float:
                    element.floatValue = value;
                    break;

                case SerializedPropertyType.Integer:
                    element.intValue = Mathf.RoundToInt(value);
                    break;
            }
        }

        targetProperty.serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(targetProperty.serializedObject.targetObject);
        AssetDatabase.SaveAssets();
        Close();
    }

    private float CalculateValue(int index)
    {
        float calculatedValue = 0f;
        switch (selectedGraph)
        {
            case EditorGraph.Linear:
                calculatedValue = linearStartValue + (linearIncrement * index);
                break;
            case EditorGraph.Step:
                calculatedValue = stepStartValue + (stepIncrement * (index / stepInterval));
                break;
            case EditorGraph.Log:
                calculatedValue = logStartValue * Mathf.Log(index + 1, logFactor); // index + 1로 0 방지
                break;
        }
        return calculatedValue;
    }
}