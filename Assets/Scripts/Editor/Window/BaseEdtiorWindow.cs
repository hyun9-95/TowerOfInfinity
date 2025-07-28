#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public abstract class BaseEdtiorWindow : EditorWindow
    {
        private Dictionary<string, object> parameters = new();
        private Dictionary<string, object> defaultParameters = new();
        private HashSet<string> labels = new();
        private Dictionary<string, object> conditionalParameters = new();
        private Dictionary<string, string> conditionalNameDic = new();
        private Dictionary<string, object> conditionalValueDic = new();
        private Dictionary<string, Type> useEnumTypes = new();
        private float spacing;

        /// <summary> Window ������ �ʱ�ȭ .. spacing - ���� ���� ���� </summary>
        protected void InitializeWindow(BaseEdtiorWindow window, float width, float height, float spacingValue)
        {
            window.minSize = new Vector2(width, height);
            window.maxSize = new Vector2(width * 2, height);
            spacing = spacingValue;
        }

        /// <summary> AddParameter�� ����� �������� �߰��ϵ��� ���� </summary>
        protected abstract void InitializeParameters();

        protected abstract void DrawActionButton();

        protected void AddParameter(string name, object defaultValue, Type enumType = null, bool setDefault = false)
        {
            if (enumType != null)
            {
                defaultParameters[name] = (Enum)defaultValue;
                useEnumTypes[name] = enumType;
            }
            else
            {
                defaultParameters[name] = defaultValue;
            }
        }

        protected void AddLabel(string desc)
        {
            labels.Add(desc);
            defaultParameters[desc] = desc;
        }

        protected void AddConditionalParameter(string name, object defaultValue, string conditionalName, object conditionalValue, Type enumType = null)
        {
            if (enumType != null)
            {
                defaultParameters[name] = (Enum)defaultValue;
                conditionalParameters[name] = (Enum)defaultValue;
                useEnumTypes[name] = enumType;
            }
            else
            {
                defaultParameters[name] = defaultValue;
                conditionalParameters[name] = defaultValue;
            }

            conditionalNameDic[name] = conditionalName;
            conditionalValueDic[name] = conditionalValue;
        }

        private void Awake()
        {
            InitializeParameters();
        }

        private void OnEnable()
        {
            SetParameterFromEditorPrefs();
        }

        private void OnGUI()
        {
            DrawParameters();
            DrawActionButton();
        }

        private void OnDisable()
        {
            SaveParameters();
        }

        private void SetParameterFromEditorPrefs()
        {
            foreach(string name in defaultParameters.Keys)
            {
                if (labels.Contains(name))
                    continue;

                Type type = defaultParameters[name].GetType();

                switch (type)
                {
                    case Type t when t == typeof(string):
                        parameters[name] = EditorPrefs.GetString(name, (string)defaultParameters[name]);
                        break;

                    case Type t when t == typeof(int):
                        parameters[name] = EditorPrefs.GetInt(name, (int)defaultParameters[name]);
                        break;

                    case Type t when t == typeof(bool):
                        parameters[name] = EditorPrefs.GetBool(name, (bool)defaultParameters[name]);
                        break;

                    default:
                        if (useEnumTypes.ContainsKey(name))
                        {
                            string enumString = EditorPrefs.GetString(name, defaultParameters[name].ToString());
                            parameters[name] = Enum.Parse(useEnumTypes[name], enumString);
                        }
                        break;
                }
            }
        }

        private void SaveParameters()
        {
            foreach (string name in defaultParameters.Keys)
            {
                if (conditionalParameters.ContainsKey(name))
                {
                    if (!CheckConditionalParameter(name))
                        continue;
                }

                if (labels.Contains(name))
                    continue;

                Type type = defaultParameters[name].GetType();

                switch (type)
                {
                    case Type t when t == typeof(string):
                        EditorPrefs.SetString(name, (string)parameters[name]);
                        break;

                    case Type t when t == typeof(int):
                        EditorPrefs.SetInt(name, (int)parameters[name]);
                        break;

                    case Type t when t == typeof(bool):
                        EditorPrefs.SetBool(name, (bool)parameters[name]);
                        break;

                    default:
                        if (useEnumTypes.ContainsKey(name))
                            EditorPrefs.SetString(name, parameters[name].ToString());
                        break;
                }
            }
        }

        private void DrawParameters()
        {
            foreach (string name in defaultParameters.Keys)
            {
                if (conditionalParameters.ContainsKey(name))
                {
                    if (!CheckConditionalParameter(name))
                        continue;
                }

                Type type = defaultParameters[name].GetType();

                if (labels.Contains(name))
                {
                    EditorGUILayout.LabelField(name);
                    continue;
                }
                else
                {
                    EditorGUILayout.LabelField(GetParameterLabel(name));
                }

                switch (type)
                {
                    case Type t when t == typeof(string):
                        parameters[name] = EditorGUILayout.TextField((string)parameters[name]);
                        break;

                    case Type t when t == typeof(int):
                        parameters[name] = EditorGUILayout.IntField((int)parameters[name]);
                        break;

                    case Type t when t == typeof(bool):
                        parameters[name] = EditorGUILayout.Toggle((bool)parameters[name]);
                        break;

                    default:
                        if (useEnumTypes.ContainsKey(name))
                            parameters[name] = EditorGUILayout.EnumPopup(useEnumTypes[name].Name, (Enum)parameters[name]);
                        break;
                }

                EditorGUILayout.Space(spacing);
            }
        }

        private string GetParameterLabel(string name)
        {
            //�Ľ�Į ���̽��� �ۼ��� name ���� ���̿� ���� �߰�
            //ex)LocalDataJson => Local Data Json
            return System.Text.RegularExpressions.Regex.Replace(name, @"(\p{Ll})(\p{Lu})", "$1 $2");
        }

        /// <summary> name�� AddParameter���� �߰��� �Ͱ� �����ؾ��� </summary>
        public T GetParameter<T>(string name)
        {
            if (!parameters.ContainsKey(name))
                return default;

            return parameters[name] is T ? (T)parameters[name] : default;
        }

        /// <summary> Parameter 값을 설정합니다 </summary>
        public void SetParameter<T>(string name, T value)
        {
            if (!defaultParameters.ContainsKey(name))
            {
                Logger.Warning($"Parameter '{name}' was not added with AddParameter. Adding it to parameters only.");
            }

            parameters[name] = value;
        }

        private bool CheckConditionalParameter(string name)
        {
            if (conditionalParameters.ContainsKey(name))
            {
                string conditionalName = conditionalNameDic[name];

                if (parameters.ContainsKey(conditionalName) &&
                    parameters[conditionalName].ToString().Equals(conditionalValueDic[name].ToString()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
#endif
