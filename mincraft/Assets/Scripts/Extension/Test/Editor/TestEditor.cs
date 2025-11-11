#if UNITY_EDITOR

using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Extension.Test {
    
    [CustomEditor(typeof(MonoScript))]
    public class TestEditor: TestEditorBase {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            var flag = BindingFlags.Static
                       | BindingFlags.FlattenHierarchy
                       | BindingFlags.Public
                       | BindingFlags.NonPublic;

            var targetType = (target as MonoScript)!.GetClass();
            if (targetType == null)
                return;
            
            var targets = targetType
                .HaveAttributeMethods(flag, new MethodComparer<TestMethodAttribute>())
                .OrderByDescending(data => data.Item2.Priority);

            if (!targets.Any())
                return;
            
            isFoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(isFoldOut, "TestMethods");
            if (!isFoldOut)
                return;

            
            foreach (var targetButton in targets) {

                SetPropertyField(targetButton.Method);

                var buttonName = targetButton.Attribute.Name;
                if (string.IsNullOrEmpty(buttonName))
                    buttonName = targetButton.Method.Name;
                
                if (!Application.isPlaying && targetButton.Attribute.RuntimeOnly)
                    EditorGUI.BeginDisabledGroup(true);
                
                if (GUILayout.Button(buttonName)) {

                    targetButton.Method.Invoke(null, GetParameterValue(targetButton.Method));
                }
                
                if (!Application.isPlaying && targetButton.Attribute.RuntimeOnly)
                    EditorGUI.EndDisabledGroup();

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

    }
}
#endif