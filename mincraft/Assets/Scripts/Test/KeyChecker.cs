using System;
using System.Collections.Generic;
using UnityEngine;

namespace Test {
    public class KeyChecker: MonoBehaviour {
        private void Update() {
            var pressed = new List<KeyCode>(); 
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode))) {
                if (Input.GetKeyDown(keyCode))
                    pressed.Add(keyCode);
            }
            Debug.Log(string.Join(',', pressed));
        }
    }
}