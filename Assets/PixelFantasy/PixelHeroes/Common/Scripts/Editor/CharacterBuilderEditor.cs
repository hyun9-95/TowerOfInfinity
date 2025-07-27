using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using UnityEditor;
using UnityEngine;

namespace Assets.PixelFantasy.PixelHeroes.Common.Scripts.Editor
{
    /// <summary>
    /// Adds "Rebuild" button to CharacterBuilder script.
    /// </summary>
    [CustomEditor(typeof(CharacterScripts.CharacterBuilder))]
    public class CharacterBuilderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Rebuild"))
            {
                ((CharacterScripts.CharacterBuilder)target).Rebuild();
            }
        }
    }
}