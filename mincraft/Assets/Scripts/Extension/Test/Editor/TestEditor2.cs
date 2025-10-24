#if UNITY_EDITOR

using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Extension.Test {
    
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class TestEditor2: TestEditorBase {
        
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            
            var flag = BindingFlags.Instance
                       | BindingFlags.Static
                       | BindingFlags.Public
                       | BindingFlags.NonPublic;
            
            //Find functions
            var targets = 
                ExAttribute.HaveAttributeMethods<TestMethod>(target, flag)
                    .OrderByDescending(data => data.Attribute.Priority);

            //If it didn't have attribute Field, reutrn;
            if (!targets.Any())
                return;
            
            //Show foldOut
            isFoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(isFoldOut, "TestFunction");
            if (!isFoldOut)
                return;
            
            //Show button
            foreach (var targetButton in targets) {

                SetPropertyField(targetButton.Method);

                //set button name
                var buttonName = targetButton.Attribute.Name;
                if (string.IsNullOrEmpty(buttonName))
                    buttonName = targetButton.Method.Name;

                if (!Application.isPlaying && targetButton.Attribute.RuntimeOnly) {
                    
                    EditorGUI.BeginDisabledGroup(true);
                    buttonName += "(RuntimeOnly)";
                }
                
                //button plate
                if (GUILayout.Button(buttonName)) {

                    var parameterList = GetParameterValue(targetButton.Method);
                    
                    if(targetButton.Method.IsStatic)
                        targetButton.Method.Invoke(null, parameterList);
                    else 
                        targetButton.Method.Invoke(target, parameterList);
                }
                
                if (!Application.isPlaying && targetButton.Attribute.RuntimeOnly)
                    EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
    }
}
#endif