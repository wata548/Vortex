using System;
using System.Linq;
using UnityEngine;
using Object = System.Object;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
namespace Extension.Serialize {
    [CustomPropertyDrawer(typeof(WhenValueChangeAttribute))]
    public class WhenValueChangePropertyDrawer: PropertyDrawer {

        private bool _initial = true;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            if (_initial) {
                _initial = false;
                Invoke(property);
            }
            
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);
            if (!EditorGUI.EndChangeCheck())
                return;

            Invoke(property);
        }

        private void Invoke(SerializedProperty property) {
            
            var flag = BindingFlags.Public 
                       | BindingFlags.NonPublic 
                       | BindingFlags.Instance 
                       | BindingFlags.Static; 
            
            var targetObject = property.serializedObject.targetObject;
            var targetObjectType = targetObject.GetType();
            var actionName = (attribute as WhenValueChangeAttribute)?.ActionName;
            if (actionName == null)
                return;
            
            var method = targetObjectType.GetMethod(actionName!, flag);
            if (method == null || method.GetParameters().Any())
                throw new MethodAccessException($"{actionName} isn't exist method");

            property.serializedObject.ApplyModifiedProperties();
            method!.Invoke(targetObject, new Object[] { });
        }
    }
}
#endif

namespace Extension.Serialize {
    
    public class WhenValueChangeAttribute : PropertyAttribute {

        public readonly string ActionName;
        
        /// <summary>
        /// invokeFunction can't have parameters
        /// <code>void A() { //contents... }</code>
        /// </summary>
        public WhenValueChangeAttribute(string invokeFuncName) => 
            ActionName = invokeFuncName;
    }
}