using System;
using System.Collections.Generic;
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
        private float lastUpdateTime;

        private float animationSpeed = 1f;
        private float clipFrameRate = 30f;

        private string[] stateNames;
        private int selectedStateIndex;
        private string lastPlayedState = "";

        private AnimatorState selectedState;
        private Motion selectedMotion;
        private AnimationClip selectedClip;

        private readonly List<AnimatorState> stateList = new();
        private readonly List<string> statePathList = new();

        [MenuItem("Window/Animator Preview Tool")]
        public static void ShowWindow()
        {
            GetWindow<AnimatorPreviewEditorWindow>("Animator Preview");
        }

        private void OnGUI()
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
                EditorGUILayout.HelpBox("할당된 Animator가 없습니다.", MessageType.Info);
                return;
            }

            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("플레이 모드에서는 사용할 수 없습니다.", MessageType.Warning);
                return;
            }

            if (animatorController == null)
            {
                EditorGUILayout.HelpBox("AnimatorController를 찾을 수 없습니다.", MessageType.Warning);
                return;
            }

            if (stateNames == null || stateNames.Length == 0)
            {
                EditorGUILayout.HelpBox("애니메이터에 사용 가능한 상태가 없습니다.", MessageType.Warning);
                return;
            }

            int newSelectedStateIndex = EditorGUILayout.Popup("Animation State", selectedStateIndex, stateNames);
            if (newSelectedStateIndex != selectedStateIndex)
            {
                selectedStateIndex = newSelectedStateIndex;
                CacheSelectedState();
                StopPreviewInternal(resetTimeOnly: true);
            }

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.FloatField("FPS (Clip)", clipFrameRate);
            }
            animationSpeed = EditorGUILayout.Slider("Preview Speed", animationSpeed, 0.1f, 2.5f);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(isPlaying && !isPaused ? "Pause" : (isPaused ? "Resume" : "Play")))
            {
                if (isPlaying && !isPaused) PausePreview();
                else if (isPaused) ResumePreview();
                else PlayPreview();
            }

            using (new EditorGUI.DisabledScope(!(isPlaying || isPaused)))
            {
                if (GUILayout.Button("Stop"))
                    StopPreview();
            }

            using (new EditorGUI.DisabledScope(!(isPlaying || isPaused)))
            {
                if (GUILayout.Button("Reset"))
                {
                    animationTime = 0f;
                    UpdateAnimationPreview(forceRebind: true);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (isPlaying || isPaused)
            {
                float clipLength = GetEffectiveClipLength();
                float normalizedTime = clipLength > 0f ? (animationTime / clipLength) : 0f;

                float newNormalizedTime = EditorGUILayout.Slider("Animation Time", normalizedTime, 0f, 1f);
                EditorGUILayout.LabelField($"Current Time: {animationTime:F3}s / {clipLength:F3}s");
                EditorGUILayout.LabelField($"Normalized: {normalizedTime:F3}");

                if (Mathf.Abs(newNormalizedTime - normalizedTime) > 0.001f)
                {
                    animationTime = newNormalizedTime * Mathf.Max(clipLength, 0.0001f);
                    UpdateAnimationPreview(forceRebind: false);
                }
            }
        }

        private void Update()
        {
            if (isPlaying && !isPaused && targetAnimator != null)
            {
                float now = (float)EditorApplication.timeSinceStartup;
                if (lastUpdateTime <= 0f) lastUpdateTime = now;

                float delta = (now - lastUpdateTime) * animationSpeed;
                lastUpdateTime = now;

                float length = Mathf.Max(GetEffectiveClipLength(), 0.0001f);
                animationTime += delta;
                if (animationTime >= length) animationTime %= length;

                UpdateAnimationPreview(forceRebind: false);
                Repaint();
            }
        }

        private void OnAnimatorChanged()
        {
            StopPreview();

            animatorController = null;
            stateNames = null;
            stateList.Clear();
            statePathList.Clear();
            selectedStateIndex = 0;
            selectedState = null;
            selectedMotion = null;
            selectedClip = null;

            if (targetAnimator == null) return;

            animatorController = targetAnimator.runtimeAnimatorController as AnimatorController;
            if (animatorController == null) return;

            ExtractAllStates(animatorController);
            BuildStateNameArray();
            CacheSelectedState();
        }

        private void ExtractAllStates(AnimatorController ac)
        {
            stateList.Clear();
            statePathList.Clear();

            if (ac.layers == null || ac.layers.Length == 0) return;

            var rootSM = ac.layers[0].stateMachine;
            CollectStatesRecursive(rootSM, string.Empty);
        }

        private void CollectStatesRecursive(AnimatorStateMachine sm, string currentPath)
        {
            foreach (var child in sm.states)
            {
                var state = child.state;
                string path = string.IsNullOrEmpty(currentPath) ? state.name : $"{currentPath}/{state.name}";

                stateList.Add(state);
                statePathList.Add(path);
            }

            foreach (var sub in sm.stateMachines)
            {
                var subSM = sub.stateMachine;
                string subPath = string.IsNullOrEmpty(currentPath) ? subSM.name : $"{currentPath}/{subSM.name}";
                CollectStatesRecursive(subSM, subPath);
            }
        }

        private void BuildStateNameArray()
        {
            stateNames = statePathList.ToArray();
            selectedStateIndex = Mathf.Clamp(selectedStateIndex, 0, Mathf.Max(0, stateNames.Length - 1));
        }

        private void CacheSelectedState()
        {
            if (stateList.Count > 0 && selectedStateIndex >= 0 && selectedStateIndex < stateList.Count)
            {
                selectedState = stateList[selectedStateIndex];
                selectedMotion = selectedState != null ? selectedState.motion : null;
                selectedClip = selectedMotion as AnimationClip;
                clipFrameRate = selectedClip != null ? selectedClip.frameRate : 30f;
            }
            else
            {
                selectedState = null;
                selectedMotion = null;
                selectedClip = null;
                clipFrameRate = 30f;
            }
        }

        private void PlayPreview()
        {
            if (targetAnimator == null || selectedState == null) return;

            EnsureAnimationModeStarted();

            isPlaying = true;
            isPaused = false;
            lastUpdateTime = (float)EditorApplication.timeSinceStartup;
            animationTime = 0f;
            lastPlayedState = "";

            PrepareAnimatorForPreview(forceRebind: true);
            UpdateAnimationPreview(forceRebind: true);
        }

        private void PausePreview() => isPaused = true;

        private void ResumePreview()
        {
            if (!isPaused) return;
            lastUpdateTime = (float)EditorApplication.timeSinceStartup;
            isPaused = false;
        }

        private void StopPreview()
        {
            StopPreviewInternal(resetTimeOnly: false);
        }

        private void StopPreviewInternal(bool resetTimeOnly)
        {
            isPlaying = false;
            isPaused = false;
            lastUpdateTime = 0f;
            lastPlayedState = "";

            if (!resetTimeOnly)
                animationTime = 0f;

            if (AnimationMode.InAnimationMode())
                AnimationMode.StopAnimationMode();

            SceneView.RepaintAll();
        }

        private void UpdateAnimationPreview(bool forceRebind)
        {
            if (targetAnimator == null || selectedState == null) return;

            EnsureAnimationModeStarted();
            if (forceRebind) PrepareAnimatorForPreview(forceRebind: true);

            float length = Mathf.Max(GetEffectiveClipLength(), 0.0001f);
            float normalized = (animationTime / length) % 1f;

            if (selectedClip != null)
            {
                AnimationMode.BeginSampling();
                AnimationMode.SampleAnimationClip(targetAnimator.gameObject, selectedClip, normalized * selectedClip.length);
                AnimationMode.EndSampling();
                lastPlayedState = selectedState.name;
            }
            else
            {
                AnimationMode.BeginSampling();

                if (forceRebind || lastPlayedState != selectedState.name)
                {
                    targetAnimator.Rebind();
                    targetAnimator.Update(0f);
                }

                targetAnimator.Play(selectedState.name, 0, normalized);
                targetAnimator.Update(0f);

                AnimationMode.EndSampling();
                lastPlayedState = selectedState.name;
            }

            SceneView.RepaintAll();
        }

        private void EnsureAnimationModeStarted()
        {
            if (!AnimationMode.InAnimationMode())
                AnimationMode.StartAnimationMode();
        }

        private void PrepareAnimatorForPreview(bool forceRebind)
        {
            if (forceRebind)
            {
                targetAnimator.Rebind();
                targetAnimator.Update(0f);
            }

            targetAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            targetAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        private float GetEffectiveClipLength()
        {
            if (selectedClip != null)
                return Mathf.Max(selectedClip.length, 0.0001f);

            return 1f;
        }

        private void OnDisable() => StopPreview();
        private void OnDestroy() => StopPreview();
    }
}
