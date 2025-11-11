#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Extension.Test {
    public abstract class TestEditorBase: Editor {
        
        protected bool isFoldOut = true;
        private Dictionary<MethodInfo, List<(ParameterInfo Info, string Value)>> parameters = new();
        
        protected void SetPropertyField(MethodInfo method) {
            
            parameters.TryAdd(method, new());
            int idx = 0;
                
            foreach (var parameter in method.GetParameters()) {
                var input = "";
                    

                if (idx < parameters[method].Count) {

                    var content = parameters[method][idx].Value;
                    input = EditorGUILayout.TextField($"{parameter.Name}({parameter.ParameterType.Name})", content);
                    parameters[method][idx] = (parameter, input);
                }
                else {
                    input = EditorGUILayout.TextField($"{parameter.Name}({parameter.ParameterType.Name})", parameter.HasDefaultValue ? parameter.DefaultValue!.ToString() : "");
                    parameters[method].Add((parameter, input));
                }

                idx++;
            }
        }

        protected object[] GetParameterValue(MethodInfo method) {
            var parameterList = new List<Object>();
                    
            foreach (var parameter in parameters[method]) {

                var parameterType = parameter.Info.ParameterType;

                object parsedValue = ExParse.ParseToObject(parameterType, parameter.Value);
                parameterList.Add(parsedValue);
            }

            return parameterList.ToArray();
        }
    }
}
#endif