#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public class ScriptGenerateorEditorWindow : BaseEditorWindow
    {
        private const float width = 400f;
        private const float height = 200f;
        private const float spacing = 5f;
        private ScriptGenerator scriptGenerator = null;

        private ScriptGenerator.ScriptType ScriptType => GetParameter<ScriptGenerator.ScriptType>("ScriptType");
        private ScriptGenerator.ManagerType ManagerType => GetParameter<ScriptGenerator.ManagerType>("ManagerType");
        private string ContentsName => GetParameter<string>("ContentsName");
        private bool IsPopup => GetParameter<bool>("IsPopup");

        [MenuItem("Tools/Code Generation/Script Generator")]
        public static void OpenScriptGenerateorWindow()
        {
            ScriptGenerateorEditorWindow window = (ScriptGenerateorEditorWindow)GetWindow(typeof(ScriptGenerateorEditorWindow));
            window.InitializeWindow(window, width, height, spacing);
        }

        protected override void DrawActionButton()
        {
            if (GUILayout.Button("Generate", GUILayout.Height(50)))
            {
                if (string.IsNullOrEmpty(ContentsName))
                {
                    Logger.Error("컨텐츠 이름을 입력해주세요");
                    return;
                }

                if (scriptGenerator == null)
                {
                    scriptGenerator = new();

                    switch (ScriptType)
                    {
                        case ScriptGenerator.ScriptType.Manager:
                            scriptGenerator.GenerateManager(ManagerType, ContentsName);
                            break;

                        case ScriptGenerator.ScriptType.MVC:
                            scriptGenerator.GenerateMVC(ContentsName, IsPopup);
                            break;

                        default:
                            scriptGenerator.Generate(ScriptType, ContentsName);
                            break;
                    }

                    Close();
                }
            }
        }

        protected override void InitializeParameters()
        {
            //컨텐츠네임 잘못 입력하면 스크립트 날라갈 수도 있어서 초기화
            EditorPrefs.SetString("ContentsName", "");

            AddParameter("ScriptType", ScriptGenerator.ScriptType.MVC, typeof(ScriptGenerator.ScriptType));

            AddConditionalParameter("ManagerType",
                ScriptGenerator.ManagerType.Base,
                "ScriptType",
                ScriptGenerator.ScriptType.Manager,
                typeof(ScriptGenerator.ManagerType));

            AddConditionalParameter("IsPopup", 
                false, 
                "ScriptType", 
                ScriptGenerator.ScriptType.MVC);

            AddLabel("[스크립트 이름 형식] - {ContentsName}{ScriptType}.cs");

            AddParameter("ContentsName", "");
        }
    }
}
#endif
