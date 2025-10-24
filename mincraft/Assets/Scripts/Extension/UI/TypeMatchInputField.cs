using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Extension.UI.CustomInputField {
    public class TypeMatchInputField: MonoBehaviour {

        //==================================================||Constant 
        private static readonly ReadOnlyDictionary<Type, string> inputFieldTypeRule = new(
            new Dictionary<Type, string>() {
                {typeof(int), @"^-?([0-9]*)$"},
                {typeof(uint), @"^([0-9]*)$"},
                {typeof(float), @"^-?([0-9]*)(?:\.[0-9]*)?$"},
                {typeof(string), @"^(.*)$"},
            }
        );

        private const int LengthLimit = 4;
        
        //==================================================||Serialize fields 
        [SerializeField] protected TMP_InputField _input;
        
        //==================================================||Fields

        private int lengthLimit = LengthLimit;
        private string _prevValue = "";
        protected Type _targetType = typeof(string); 
        
        //==================================================||Methods 

        public void SetType(Type targetType, int limit = LengthLimit) {

            if (!inputFieldTypeRule.ContainsKey(targetType))
                targetType = typeof(string);
            
            _targetType = targetType;

            if (!Regex.IsMatch(_input.text, inputFieldTypeRule[_targetType])) {
                _input.text = "";
                OnChange();
            }

            lengthLimit = limit;
        }
        
        public void OnChange(string t = "") {

            bool isMatch = Regex.IsMatch(_input.text, inputFieldTypeRule[_targetType]);
            bool isUnableSpace = 
                _targetType != typeof(string) 
                && _input.text.Length > 0
                && char.IsWhiteSpace(_input.text[^1]);
            Debug.Log(Regex.Match(_input.text, inputFieldTypeRule[_targetType]).Groups[1].Value);
            
            bool limitCondition = Regex.Match(_input.text, inputFieldTypeRule[_targetType])
                .Groups[1].Value.Length > lengthLimit; 
            
            
            if (!isMatch || isUnableSpace || limitCondition) {
                _input.text = _prevValue;
                return;
            }

            _prevValue = _input.text;
        } 

    }
}