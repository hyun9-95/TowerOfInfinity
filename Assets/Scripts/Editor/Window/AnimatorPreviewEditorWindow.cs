using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace TowerOfInfinity.Editor
{
    public class AnimatorPreviewEditorWindow : EditorWindow
    {
        private Animator targetAnimator;
        private AnimatorController animatorController;
        private bool isPlaying;
        private bool isPaused;
        private float animationTime;
        private double startTime;
        private double pausedTime;
        private string[] stateNames;
        private int selectedStateIndex;
        private AnimatorStateInfo currentStateInfo;
        private AnimationClip currentClip;

        [MenuItem("Window/Animator Preview Tool")]
        public static void ShowWindow()
        {
            GetWindow<AnimatorPreviewEditorWindow>("Animator Preview");
        }

        void OnGUI()
        {
            GUILayout.Label("Animator Preview Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            targetAnimator = (Animator)EditorGUILayout.ObjectField("Target Animator", targetAnimator, typeof(Animator), true);
            
            if (EditorGUI.EndChangeCheck())
            {
                OnAnimatorChanged();
            }

            if (targetAnimator == null)
            {
                EditorGUILayout.HelpBox("할당된 애니메이터가 없습니다.", MessageType.Info);
                return;
            }

            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("플레이 모드에서는 사용할 수 없습니다.", MessageType.Warning);
                return;
            }

            EditorGUILayout.Space();

            if (stateNames != null && stateNames.Length > 0)
            {
                selectedStateIndex = EditorGUILayout.Popup("Animation State", selectedStateIndex, stateNames);
                
                EditorGUILayout.Space();
                
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button(isPlaying && !isPaused ? "Pause" : (isPaused ? "Resume" : "Play")))
                {
                    if (isPlaying && !isPaused)
                        PausePreview();
                    else if (isPaused)
                        ResumePreview();
                    else
                        PlayPreview();
                }
                
                if (isPlaying || isPaused)
                {
                    if (GUILayout.Button("Stop"))
                    {
                        StopPreview();
                    }
                }
                
                GUI.enabled = isPlaying || isPaused;
                if (GUILayout.Button("Reset"))
                {
                    animationTime = 0f;
                    UpdateAnimationPreview();
                }
                GUI.enabled = true;
                
                EditorGUILayout.EndHorizontal();

                if (isPlaying || isPaused)
                {
                    EditorGUILayout.Space();
                    
                    float clipLength = currentClip != null ? currentClip.length : currentStateInfo.length;
                    float actualDuration = clipLength;
                    float normalizedTime = animationTime / actualDuration;
                    float newNormalizedTime = EditorGUILayout.Slider("Animation Time", normalizedTime, 0f, 1f);
                    
                    EditorGUILayout.LabelField($"Current Time: {animationTime:F3}s / {clipLength:F3}s");
                    
                    if (Mathf.Abs(newNormalizedTime - normalizedTime) > 0.001f)
                    {
                        animationTime = newNormalizedTime * actualDuration;
                        UpdateAnimationPreview();
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("애니메이터에 사용 가능한 상태가 없습니다.", MessageType.Warning);
            }
        }

        void Update()
        {
            if (isPlaying && !isPaused && targetAnimator != null)
            {
                double currentTime = EditorApplication.timeSinceStartup;
                animationTime = (float)(currentTime - startTime);
                
                float clipLength = currentClip != null ? currentClip.length : currentStateInfo.length;
                float actualDuration = clipLength;
                
                if (actualDuration > 0 && animationTime >= actualDuration)
                {
                    startTime = currentTime;
                    animationTime = 0f;
                }
                
                UpdateAnimationPreview();
                Repaint();
            }
        }

        private void OnAnimatorChanged()
        {
            StopPreview();
            
            if (targetAnimator == null)
            {
                stateNames = null;
                return;
            }

            animatorController = targetAnimator.runtimeAnimatorController as AnimatorController;
            
            if (animatorController == null)
            {
                stateNames = null;
                return;
            }

            ExtractStateNames();
        }

        private void ExtractStateNames()
        {
            if (animatorController == null || animatorController.layers.Length == 0)
            {
                stateNames = null;
                return;
            }

            var states = animatorController.layers[0].stateMachine.states;
            stateNames = new string[states.Length];
            
            for (int i = 0; i < states.Length; i++)
            {
                stateNames[i] = states[i].state.name;
            }
            
            selectedStateIndex = 0;
        }

        private void PlayPreview()
        {
            if (targetAnimator == null || stateNames == null || selectedStateIndex >= stateNames.Length)
                return;

            isPlaying = true;
            isPaused = false;
            startTime = EditorApplication.timeSinceStartup;
            animationTime = 0f;
            
            string stateName = stateNames[selectedStateIndex];
            targetAnimator.Play(stateName, 0, 0f);
            
            AnimatorClipInfo[] clipInfos = targetAnimator.GetCurrentAnimatorClipInfo(0);
            if (clipInfos.Length > 0)
            {
                currentStateInfo = targetAnimator.GetCurrentAnimatorStateInfo(0);
                currentClip = clipInfos[0].clip;
            }
            
            targetAnimator.Update(0f);
        }

        private void StopPreview()
        {
            isPlaying = false;
            isPaused = false;
            animationTime = 0f;
        }

        private void PausePreview()
        {
            isPaused = true;
            pausedTime = EditorApplication.timeSinceStartup;
        }

        private void ResumePreview()
        {
            if (isPaused)
            {
                double pauseDuration = EditorApplication.timeSinceStartup - pausedTime;
                startTime += pauseDuration;
            }
            isPaused = false;
        }

        private void UpdateAnimationPreview()
        {
            if (targetAnimator == null || (!isPlaying && !isPaused))
                return;

            float clipLength = currentClip != null ? currentClip.length : currentStateInfo.length;
            float normalizedTime = animationTime / clipLength;
            
            targetAnimator.Play(stateNames[selectedStateIndex], 0, normalizedTime % 1f);
            targetAnimator.Update(0f);
        }

        void OnDisable()
        {
            StopPreview();
        }
    }
}