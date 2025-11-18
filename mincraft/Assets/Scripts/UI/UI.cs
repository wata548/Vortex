using System;
using Entity;
using MapGenerator;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace {
    public class UI: MonoBehaviour {

        [SerializeField] private TMP_Text _inputTypeShower;
        [SerializeField] private TMP_Text _sensitivityShower;
        [SerializeField] private GameObject _pannel;
        [SerializeField] private Scrollbar _scrollbar;
        private bool _isKeyboardType = true;
        
        
        public void Continue() {
            Time.timeScale = 1;
        }

        public void InputTypeChange() {
            _isKeyboardType = !_isKeyboardType;
            var type = _isKeyboardType ? "Keyboard" : "Controller";
            _inputTypeShower.text = $"Control Mode: {type}";
            var keyMap = Resources.Load<ScriptableInputSetting>($"{type}KeyMap");
            PlayerEntity.Instance.Movement.SetKeyMap(keyMap);
        }

        public void ChangeCameraSensitivity() {
            const float MIN = 0.1f;
            const float MAX = 3f;

            var value = _scrollbar.value;
            value *= MAX - MIN;
            value += MIN;
            PlayerEntity.Instance.Movement.SetCameraSensitivity(value);
            _sensitivityShower.text = $"{value:N2}";
        }

        public void Quit() {
            Application.Quit();
        }
        public void ToTitle() {
            SceneManager.LoadScene("Main");
        }

        private void Update() {
            var isOn = Time.timeScale == 0;
            _pannel.SetActive(isOn);
            Cursor.visible = isOn;
            Cursor.lockState = isOn ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}